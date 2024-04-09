using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace TasksManagement_API.Interfaces
{
    public interface ICertificateValidationService
    {
        internal bool ValidateCertificate(X509Certificate2 clientCertificate);
    }
}