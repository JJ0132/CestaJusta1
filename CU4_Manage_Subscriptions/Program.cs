using CestaJusta.CU4.ManageSubscriptions.Application;
using CestaJusta.CU4.ManageSubscriptions.Infrastructure;
using CestaJusta.CU4.ManageSubscriptions.Domain;

namespace CestaJusta.CU4.ManageSubscriptions;

internal static class Program
{
    private static async Task Main()
    {
        Console.WriteLine("=== CASO DE USO 4: GESTION DE SUSCRIPCIONES ===\n");

        string connectionString = Environment.GetEnvironmentVariable("CESTAJUSTA_CONNECTION_STRING")
            ?? "Server=localhost\\SQLEXPRESS;Database=MercadonaDB;Trusted_Connection=True;TrustServerCertificate=True;";

        var repository = new SqlServerSubscriptionRepository(connectionString);
        var paymentGateway = new ConsolePaymentGateway();
        var useCase = new ManageSubscriptionUseCase(repository, paymentGateway);

        while (true)
        {
            ManageSubscriptionRequest request = ReadRequestFromConsole();
            ManageSubscriptionResult result = await useCase.ExecuteAsync(request);

            Console.WriteLine();
            if (result.Success)
            {
                Console.WriteLine("Suscripcion actualizada correctamente.");
                Console.WriteLine($"Perfil: {result.ProfileId}");
                Console.WriteLine($"Plan anterior: {result.PreviousPlan}");
                Console.WriteLine($"Plan nuevo: {result.NewPlan}");
                Console.WriteLine($"Privilegios: {string.Join(", ", result.GrantedPrivileges)}");
                Console.WriteLine($"Recibo: {result.Receipt?.ReceiptNumber}");
            }
            else
            {
                Console.WriteLine($"Error: {result.Message}");
            }

            Console.WriteLine();
            Console.Write("¿Quieres gestionar otra suscripcion? (s/n): ");
            string? answer = Console.ReadLine();
            if (!string.Equals(answer, "s", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            Console.WriteLine();
        }
    }

    private static ManageSubscriptionRequest ReadRequestFromConsole()
    {
        Console.Write("Id de perfil: ");
        int profileId = int.TryParse(Console.ReadLine(), out int parsedProfileId) ? parsedProfileId : 0;

        Console.WriteLine("Plan destino:");
        Console.WriteLine("1 - Basic (gratis)");
        Console.WriteLine("2 - Plus (2.99 EUR)");
        Console.WriteLine("3 - Familiar (5.99 EUR)");
        Console.Write("Seleccion: ");

        string? selectedPlan = Console.ReadLine();
        SubscriptionPlan plan = selectedPlan switch
        {
            "2" => SubscriptionPlan.Plus,
            "3" => SubscriptionPlan.Familiar,
            _ => SubscriptionPlan.Basic
        };

        Console.Write("Titular de la tarjeta: ");
        string cardholderName = Console.ReadLine() ?? string.Empty;

        Console.Write("Referencia de pago: ");
        string paymentReference = Console.ReadLine() ?? string.Empty;

        Console.Write("Aprobar pago en la pasarela? (s/n): ");
        bool approvePayment = string.Equals(Console.ReadLine(), "s", StringComparison.OrdinalIgnoreCase);

        PaymentDetails paymentDetails = new(
            CardholderName: cardholderName,
            PaymentReference: paymentReference,
            ApprovePayment: approvePayment);

        return new ManageSubscriptionRequest(profileId, plan, paymentDetails);
    }
}