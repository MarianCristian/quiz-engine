using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Qubiz.QuizEngine
{
    public static class HTMLExtensions
    {
        public static MvcHtmlString DropDownListForEnum<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
            where TProperty : struct
        {
            return htmlHelper.DropDownListFor(expression, ToSelectList<TProperty>(typeof(TProperty)), htmlAttributes);
        }

        private static SelectList ToSelectList<TEnum>(Type enumType)
        {
            var values = from TEnum e in Enum.GetValues(enumType)
                         select new { Id = e, Name = e.ToString() };
            return new SelectList(values, "Id", "Name", Enum.GetValues(enumType));
        }

        public static string EnumToJsonObject<TEnum>()
        {
          
}