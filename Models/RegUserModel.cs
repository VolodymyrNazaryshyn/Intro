namespace Intro.Models {
    // модель для сбора данных из формы регистрации пользователя, имена
    // свойств должны совпадать (до регистра) с именами полей формы  
    public class RegUserModel 
    {
        public string RealName  { get; set; }
        public string Login     { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }
        public string Email     { get; set; }
        public Microsoft.AspNetCore.Http.IFormFile Avatar { get; set; }
    }
}
