using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }


        public static void AddPagination(this HttpResponse response, int CurrentPage, int itemsPerPage, int totalItems, int TotalPages)
        {
            var paginationHeader=new PaginationHeader(CurrentPage,itemsPerPage,totalItems,TotalPages);
            var CamelCasFormatter=new JsonSerializerSettings();
            CamelCasFormatter.ContractResolver=new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination",JsonConvert.SerializeObject(paginationHeader,CamelCasFormatter));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
        public static int CalculateAge(this DateTime theDateTime)
        {
            var age = DateTime.Today.Year - theDateTime.Year;
            if (theDateTime.AddYears(age) > DateTime.Today)
                age--;
            return age;
        }
    }
}