using BlogEngineTK.Domain;
using BlogEngineTK.Domain.Entities;
using BlogEngineTK.Domain.Repositories;
using BlogEngineTK.Domain.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogEngineTK.WebUI.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private IPostRepository repository;

        public PostController(IPostRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Выводит редактор для создания нового поста
        /// </summary>
        public ActionResult Create(string returnUrl)
        {
            ViewBag.Title = "Создание нового поста";
            ViewBag.ReturnUrl = returnUrl;
            return View("Editor");
        }
        
        /// <summary>
        /// Получает данные с редактора и если данные валидны, то сохраняет их
        /// </summary>
        /// <param name="model">Данные поста с редактора</param>
        /// <returns>Возвращает RedirectResult если данные успешно сохранены; 
        /// в противном случае возвращает ViewResult (редактор)</returns>
        [HttpPost]
        public ActionResult Create(Post model, string returnUrl)
        {
            // Т.к. полученный HTML код с model.ShortText | model.FullText
            // не будет выполняться на сервере, а просто храниться,
            // то не будем производить строгую валидация (разр. или запр. опред. тэги)
            // и это даст возможность пользователю делать более динамичные записи.
                        
            if (ModelState.IsValid)
            {
                // Значения по умолчанию для нового поста
                model.Tags = null;
                model.CommentsNumber = 0;                
                model.CreatedDate = DateTime.Now;
                model.Language = Language.Ru;

                repository.InsertPost(model);

                BlogEngineTK.WebUI.MvcApplication.ClearAppCache();
                return Redirect(returnUrl);
            }

            ViewBag.Title = "Создание нового поста";
            return View("Editor");
        }

        /// <summary>
        /// Выводит редактор для редактирования сущ-го поста
        /// </summary>
        public ActionResult Update(int postId, string returnUrl)
        {
            Post p = repository.Posts.FirstOrDefault(x => x.PostId == postId);

            if (p == null)
            {
                return Redirect(returnUrl);
            }
            else
            {
                ViewBag.Title = "Редактирование поста";
                ViewBag.ReturnUrl = returnUrl;
                return View("Editor", p);
            }
        }

        /// <summary>
        /// Получает данные с редактора и если данные валидны, то сохраняет их
        /// </summary>
        /// <param name="model">Данные поста с редактора</param>
        /// <returns>Возвращает RedirectResult если данные успешно сохранены; 
        /// в противном случае возвращает ViewResult (редактор)</returns>
        [HttpPost]
        public ActionResult Update(Post model, string returnUrl)
        {
            // Т.к. полученный HTML код с model.ShortText | model.FullText
            // не будет выполняться на сервере, а просто храниться,
            // то не будем производить строгую валидация (разр. или запр. опред. тэги)
            // и это даст возможность пользователю делать более динамичные записи.

            if (ModelState.IsValid)
            {
                repository.UpdatePost(model);

                BlogEngineTK.WebUI.MvcApplication.ClearAppCache();
                return Redirect(returnUrl);
            }

            ViewBag.Title = "Редактирование поста";
            return View("Editor", model);
        }

        /// <summary>
        /// Выводит диалоговое окно для подтверждения или отмены удаления поста
        /// </summary>
        /// <param name="postId">ID удаляемого поста</param>
        /// <returns>Возвращает RedirectResult если пост с таким ID не найден; 
        /// в противном случае выводит диалоговое окно</returns>
        public ActionResult Delete(int postId, string returnUrl)
        {
            Post p = repository.Posts.FirstOrDefault(x => x.PostId == postId);

            if (p == null)
            {
                return Redirect(returnUrl);
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(p);
            }            
        }

        /// <summary>
        /// Производит подтвержденное удаления поста по ID
        /// </summary>
        /// <param name="post">Пост в котором указано ID</param>
        /// <returns>Возвращает RedirectResult если пост успешно удален; 
        /// в противном случае возвращает null</returns>
        [HttpPost]
        public ActionResult DeletePost(int postId, string returnUrl)
        {
            Post p = repository.Posts.FirstOrDefault(x => x.PostId == postId);

            if (p == null)
            {
                return null;
            }
            else
            {
                repository.DeletePost(postId);

                BlogEngineTK.WebUI.MvcApplication.ClearAppCache();
                return Redirect(returnUrl);
            }  
        }
    }
}
