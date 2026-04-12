using CestaJusta.CU4.ManageSubscriptions.Application.Abstractions;
using CestaJusta.CU4.ManageSubscriptions.Domain;

namespace CestaJusta.CU4.ManageSubscriptions.Application;

public sealed class ManageSubscriptionUseCase
{
    private static readonly IReadOnlyDictionary<SubscriptionPlan, SubscriptionDefinition> PlanDefinitions =
        new Dictionary<SubscriptionPlan, SubscriptionDefinition>
        {
            [SubscriptionPlan.Basic] = new(
                "Basic",
                0m,
                new[]
                {
                    "menus_semanales",
                    "ajuste_presupuesto",
                    "filtros_basicos",
                    "necesidades_medicas"
                }),
            [SubscriptionPlan.Plus] = new(
                "Plus",
                2.99m,
                new[]
                {
                    "menus_semanales",
                    "ajuste_presupuesto",
                    "filtros_basicos",
                    "necesidades_medicas",
                    "intercambio_recetas",
                    "macronutrientes_detallados"
                }),
            [SubscriptionPlan.Familiar] = new(
                "Familiar",
                5.99m,
                new[]
                {
                    "menus_semanales",
                    "ajuste_presupuesto",
                    "filtros_basicos",
                    "necesidades_medicas",
                    "intercambio_recetas",
                    "macronutrientes_detallados",
                    "multiples_perfiles",
                    "analitica_financiera",
                    "porcentaje_ahorrado",
                    "total_ahorrado"
                })
        };

    private readonly IProfileSubscriptionRepository profileRepository;
    private readonly IPaymentGateway paymentGateway;

    public ManageSubscriptionUseCase(
        IProfileSubscriptionRepository profileRepository,
        IPaymentGateway paymentGateway)
    {
        this.profileRepository = profileRepository;
        this.paymentGateway = paymentGateway;
    }

    public async Task<ManageSubscriptionResult> ExecuteAsync(
        ManageSubscriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        string? validationError = Validate(request);
        if (validationError is not null)
        {
            return ManageSubscriptionResult.Failed(validationError);
        }

        SubscriptionDefinition targetPlan = PlanDefinitions[request.TargetPlan];
        SubscriptionProfile? profile = await profileRepository.GetByIdAsync(request.ProfileId, cancellationToken);
        if (profile is null)
        {
            return ManageSubscriptionResult.Failed("No existe un perfil con ese identificador.");
        }

        if (profile.CurrentPlan == request.TargetPlan)
        {
            return ManageSubscriptionResult.Failed("El perfil ya tiene ese plan de suscripcion.");
        }

        PaymentAuthorizationResult paymentResult = await paymentGateway.AuthorizeAsync(
            request.PaymentDetails,
            targetPlan.Price,
            cancellationToken);

        if (!paymentResult.Approved)
        {
            return ManageSubscriptionResult.Failed(paymentResult.ErrorMessage ?? "Pago rechazado por la pasarela.");
        }

        DateTime utcNow = DateTime.UtcNow;
        await profileRepository.UpdateSubscriptionAsync(
            request.ProfileId,
            request.TargetPlan,
            targetPlan.Privileges,
            utcNow,
            cancellationToken);

        SubscriptionReceipt receipt = new(
            ReceiptNumber: $"RCP-{utcNow:yyyyMMddHHmmss}-{request.ProfileId}",
            ProfileId: request.ProfileId,
            PreviousPlan: profile.CurrentPlan,
            NewPlan: request.TargetPlan,
            Amount: targetPlan.Price,
            Currency: "EUR",
            PaymentReference: paymentResult.AuthorizationCode,
            IssuedAtUtc: utcNow);

        return ManageSubscriptionResult.Succeeded(
            request.ProfileId,
            profile.CurrentPlan,
            request.TargetPlan,
            targetPlan.Privileges,
            receipt);
    }

    private static string? Validate(ManageSubscriptionRequest request)
    {
        if (request.ProfileId <= 0)
        {
            return "El identificador de perfil no es valido.";
        }

        if (!PlanDefinitions.ContainsKey(request.TargetPlan))
        {
            return "El plan seleccionado no es valido.";
        }

        if (string.IsNullOrWhiteSpace(request.PaymentDetails.CardholderName))
        {
            return "El nombre del titular del pago es obligatorio.";
        }

        if (string.IsNullOrWhiteSpace(request.PaymentDetails.PaymentReference))
        {
            return "La referencia de pago es obligatoria.";
        }

        return null;
    }

    private sealed record SubscriptionDefinition(string Name, decimal Price, IReadOnlyList<string> Privileges);
}