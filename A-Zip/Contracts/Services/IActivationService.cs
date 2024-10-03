namespace A_Zip.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
