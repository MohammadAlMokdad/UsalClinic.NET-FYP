using Microsoft.AspNetCore.Mvc;

namespace UsalClinic.Web.ViewModels
{
    public class ContactFormViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
    }
}