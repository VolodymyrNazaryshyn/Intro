using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intro.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
    }
}
/*
 Д.З. Разработать методы ArticleController:
 Get(String TopicId) => коллекция статей данного раздела (топика)
 Post(Models.ArticleModel)
  TopicId  -- можно заложить в модель (ArticleModel)
  AuthorId -- в HTTP заголовке
 */