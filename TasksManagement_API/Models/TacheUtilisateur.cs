using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksManagement_API.Models
{
    public class TacheUtilisateur
    {
        public int UserId { get; set; }
        public String? NomUtilisateur { get; set; }
        public String? EmailUtilisateur { get; set; }
        public ICollection<Tache>? LesTaches { get; set; }
    }
}