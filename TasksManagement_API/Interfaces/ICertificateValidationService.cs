using System.Security.Cryptography.X509Certificates;

namespace TasksManagement_API.Interfaces
{
    public interface ICertificateValidationService
    {
        internal bool ValidateCertificate(X509Certificate2 clientCertificate);
    }
}