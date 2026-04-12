using Microsoft.Playwright;
using Microsoft.Data.SqlClient;
using Dapper;
using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;

class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Iniciando el bot...");

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var page = await browser.NewPageAsync();

        // 1. Ir a Mercadona
        await page.GotoAsync("https://tienda.mercadona.es/");

        // 2. Aceptar cookies
        try
        {
            Console.WriteLine("Buscando el botón de cookies...");
            await page.GetByRole(AriaRole.Button, new() { Name = "Aceptar" }).ClickAsync(new() { Timeout = 3000 });
        }
        catch
        {
            Console.WriteLine("No ha salido cartel de cookies, seguimos...");
        }

        // 3. Código postal
        Console.WriteLine("Introduciendo código postal...");
        var inputCodigoPostal = page.GetByTestId("postal-code-checker-input");
        await inputCodigoPostal.ClickAsync();
        await inputCodigoPostal.FillAsync("28223");
        await page.GetByTestId("postal-code-checker-button").ClickAsync();

        // 4. Navegar a la sección general de Categorías
        Console.WriteLine("Navegando al catálogo de categorías...");

        // Hacemos clic en la pestaña "Categorías"
        await page.GetByText("Categorías", new() { Exact = true }).ClickAsync();
        
        // Esperamos
        await page.WaitForSelectorAsync(".category-menu__item");

        // Obtenemos todos los botones del menú lateral izquierdo
        var categoriasLoc = page.Locator(".category-menu__item button");
        int totalCategorias = await categoriasLoc.CountAsync();
        Console.WriteLine($"¡He detectado {totalCategorias} categorías principales!");

        // Preparamos la conexión SQL
        string connectionString = "Server=localhost\\SQLEXPRESS;Database=MercadonaDB;Trusted_Connection=True;TrustServerCertificate=true;";
        using var connection = new SqlConnection(connectionString);
        string sql = @"
            IF NOT EXISTS (
                SELECT 1 FROM Precio_Historico 
                WHERE Nombre = @Nombre AND CAST(Fecha_Captura AS DATE) = CAST(@Fecha AS DATE)
            )
            BEGIN
                INSERT INTO Precio_Historico (Nombre, Precio, Fecha_Captura) 
                VALUES (@Nombre, @Precio, @Fecha)
            END";

        // Evitamos los productos duplicados
        var productosProcesados = new HashSet<string>();

        // Bucle para recorrer todas las categorias
        for (int i = 0; i < totalCategorias; i++)
        {
            // Buscamos los elementos
            var categoriaBtn = page.Locator(".category-menu__item button").Nth(i);
            
            string nombreCategoria = await categoriaBtn.InnerTextAsync();
            Console.WriteLine($"\n--- Explorando categoría: {nombreCategoria.Trim()} ---");

            await categoriaBtn.ClickAsync();
            
            // Pausa obligatoria para que no detecte el bot
            await Task.Delay(2000); 

            bool quedanSubcategorias = true;

            // Bucle para bajar por los botones
            while (quedanSubcategorias)
            {
                // 1. Extraemos todo lo que hay en pantalla en este momento
                var tarjetas = await page.GetByTestId("product-cell").AllAsync();
                
                foreach (var tarjeta in tarjetas)
                {
                    try 
                    {
                        var nombreProd = await tarjeta.GetByTestId("product-cell-name").InnerTextAsync();

                        // 2. Si el producto NO está en nuestro HashSet, es nuevo. Lo procesamos.
                        if (!productosProcesados.Contains(nombreProd))
                        {
                            var precioTexto = await tarjeta.GetByTestId("product-price").InnerTextAsync();
                            var matchPrecio = Regex.Match(precioTexto, @"[\d,]+").Value;
                            decimal precioFinal = decimal.Parse(matchPrecio, new CultureInfo("es-ES"));

                            await connection.ExecuteAsync(sql, new {
                                Nombre = nombreProd,
                                Precio = precioFinal,
                                Fecha = DateTime.Now
                            });

                            productosProcesados.Add(nombreProd);
                            Console.WriteLine($"Guardado: {nombreProd} - {precioFinal}€");
                        }
                    }
                    catch {}
                }

                // 3. Buscamos el botón verde de "Ver siguiente"
                var btnSiguiente = page.Locator(".category-detail__next-subcategory");
                
                // Si el botón es visible, lo pulsamos para cargar la siguiente sección
                if (await btnSiguiente.IsVisibleAsync())
                {
                    Console.WriteLine("Cargando más productos en esta categoría...");
                    await btnSiguiente.ClickAsync();
                    
                    // Pausa clave para que no detecte el bot
                    await Task.Delay(2000); 
                }
                else
                {
                    // Si el botón ya no está, hemos llegado al final de esta categoría principal
                    Console.WriteLine("Fin de esta categoría.");
                    quedanSubcategorias = false; 
                }
            }
        }
    }
}