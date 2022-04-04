using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SplashShoppingCart.Infrastructure
{
    public class ImgValidationFileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //var context = (SplashShoppingCartContext)validationContext.GetService(typeof(SplashShoppingCartContext));
            //context.Products.OrderByDescending(x => x.Id).Include(x => x.Category).ToListAsync());

            //value.ToString();
            //lets validate file or image we are receiving
            var file = value as IFormFile;
            if(file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                string[] extensions = { "jpg", "png" };
                bool result = extensions.Any(x => extension.EndsWith(x));

                if (!result)
                {
                    return new ValidationResult(GetErrorMessage());
                }
                return ValidationResult.Success;
            }
            //return ValidationResult.Success;
            return ValidationResult.Success;

        }
        //private string GetErrorMessage2()
        //{
        //    return "Please upload an Image";
        //}
        private string GetErrorMessage()
        {
            return "Allowed extensions are jpg and png.";
        }
    }
}
