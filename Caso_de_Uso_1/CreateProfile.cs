using System;
using System.Text.RegularExpressions;

namespace CestaJusta.CasoDeUso1
{
    public record PerfilUsuario(string Nombre, string Apellidos, string NombreUsuario, string Gmail, string Contrasena);

    public class CreateProfileUseCase
    {
        public ResultadoCreacion Ejecutar(string nombre, string apellidos, string nombreUsuario, string gmail, string contrasena)
        {
            if (!ValidarDatos(nombre, apellidos, nombreUsuario, gmail, contrasena, out string mensajeError))
            {
                return ResultadoCreacion.Fallido(mensajeError);
            }

            var perfil = new PerfilUsuario(
                Nombre: nombre.Trim(),
                Apellidos: apellidos.Trim(),
                NombreUsuario: nombreUsuario.Trim(),
                Gmail: gmail.Trim().ToLower(),
                Contrasena: contrasena
            );

            return ResultadoCreacion.Exitoso(perfil);
        }

        private bool ValidarDatos(string nombre, string apellidos, string nombreUsuario, string gmail, string contrasena, out string mensajeError)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellidos) ||
                string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(gmail) ||
                string.IsNullOrWhiteSpace(contrasena))
            {
                mensajeError = "Todos los campos son obligatorios.";
                return false;
            }

            string emailPattern = @"^[^@\s]+@gmail\.com$";
            if (!Regex.IsMatch(gmail.Trim().ToLower(), emailPattern))
            {
                mensajeError = "El correo debe ser un Gmail válido (ej: usuario@gmail.com).";
                return false;
            }

            if (contrasena.Length < 6)
            {
                mensajeError = "La contraseña debe tener al menos 6 caracteres.";
                return false;
            }

            mensajeError = string.Empty;
            return true;
        }
    }

    public class ResultadoCreacion
    {
        public bool EsValido { get; private set; }
        public string Mensaje { get; private set; } = string.Empty;
        public PerfilUsuario? Perfil { get; private set; }

        private ResultadoCreacion() { }

        public static ResultadoCreacion Exitoso(PerfilUsuario perfil)
        {
            return new ResultadoCreacion
            {
                EsValido = true,
                Perfil = perfil,
                Mensaje = "Perfil válido y listo para guardar."
            };
        }

        public static ResultadoCreacion Fallido(string mensaje)
        {
            return new ResultadoCreacion
            {
                EsValido = false,
                Perfil = null,
                Mensaje = mensaje
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== CASO DE USO 1: CREAR PERFIL DE USUARIO ===\n");

            Console.Write("Nombre: ");
            string nombre = Console.ReadLine() ?? string.Empty;

            Console.Write("Apellidos: ");
            string apellidos = Console.ReadLine() ?? string.Empty;

            Console.Write("Nombre de usuario: ");
            string nombreUsuario = Console.ReadLine() ?? string.Empty;

            Console.Write("Correo Gmail: ");
            string gmail = Console.ReadLine() ?? string.Empty;

            Console.Write("Contraseña: ");
            string contrasena = Console.ReadLine() ?? string.Empty;

            var casoUso = new CreateProfileUseCase();
            var resultado = casoUso.Ejecutar(nombre, apellidos, nombreUsuario, gmail, contrasena);

            if (resultado.EsValido)
            {
                Console.WriteLine("\nPerfil creado correctamente.\n");
                Console.WriteLine($"Nombre: {resultado.Perfil?.Nombre}");
                Console.WriteLine($"Apellidos: {resultado.Perfil?.Apellidos}");
                Console.WriteLine($"Usuario: {resultado.Perfil?.NombreUsuario}");
                Console.WriteLine($"Gmail: {resultado.Perfil?.Gmail}");
            }
            else
            {
                Console.WriteLine($"\nError: {resultado.Mensaje}");
            }
        }
    }
}
