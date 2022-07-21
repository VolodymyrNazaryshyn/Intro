using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Threading.Tasks;

namespace Intro.Middleware
{
    public class SessionAuthMiddleware
    {
        private readonly RequestDelegate _next; // обязательное поле для Middleware (ссылка на следующий слой)

        public SessionAuthMiddleware(RequestDelegate next) // обязательная форма конструктора
        {
            _next = next;
        }

        // обязательный метод класса
        public async Task InvokeAsync(HttpContext context, Services.IAuthService authService, DAL.Context.IntroContext introContext)
        {
            //var cultureFeature = context.Features.Get<IRequestCultureFeature>();
            //context.Items.Add("Locale", cultureFeature.RequestCulture.UICulture.ToString());

            String userId = context.Session.GetString("userId");
            if (userId != null)
            {
                authService.Set(userId);
                // Извлекаем из сессии метку времени начала авторизации и вычисляем длительность (авторизованного сеанса)
                long authMoment = Convert.ToInt64(context.Session.GetString("AuthMoment"));
                long authDuration = (DateTime.Now.Ticks - authMoment) / (long)1e7; // узнаем длительность нашей сессии
                if (authDuration > 100) // Предельная длительность сеанса авторизации
                {
                    // Стираем из сессии признак авторизации
                    context.Session.Remove("userId");
                    context.Session.Remove("AuthMoment");
                    // По правилам безопасности: если меняется состояние авторизации то необходимо перезагрузить систему (страницу)
                    context.Response.Redirect("/");
                    // сохраняем время выхода в базу
                    authService.User.LogMoment = DateTime.Now;
                    introContext.SaveChanges();
                    return;
                }
            }

            context.Items.Add("fromAuthMiddleware", "Hello !!");
            await _next(context);
        }
    }
}
