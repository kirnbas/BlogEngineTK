using BlogEngineTK.Domain.Entities;
using BlogEngineTK.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BlogEngineTK.WebUI.Infrastructure.HtmlHelpers
{
    public static class PagingHelpers
    {
        /// <summary>
        /// Возвращает ссылки на первые 3 страницы; на текущую, пред. и след. страницу; и на последние 3 страницы.
        /// </summary>
        /// <param name="helper">Расширяемый класс</param>
        /// <param name="pagingInfo">Инф-ция для создания линков</param>
        /// <param name="pageUrl">Лямбда-выражение для возвращение ссылки</param>
        public static MvcHtmlString GetPageLinks(this HtmlHelper helper, PagingInfo pagingInfo,
            Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            if (pagingInfo.TotalItems > 0)
            {
                result.Append("Страницы: ");
            }            
                           
            // Добавим ссылку на пред. страницу
            if (pagingInfo.CurrentPage > 1)
            {
                TagBuilder link = new TagBuilder("a");
                link.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage - 1));
                link.InnerHtml = "Пред. ";
                result.Append(link);
            }

            int page = 1; // Будет в основном исп-ться для пропуска уже добавленных ссылок

            // Добавим ссылки на первые 3? страницы
            while (page <= pagingInfo.TotalPages && page <= 3)
            {
                if (page == pagingInfo.CurrentPage)
                {
                    result.Append(page);
                }
                else
                {
                    TagBuilder link = new TagBuilder("a");
                    link.MergeAttribute("href", pageUrl(page));
                    link.InnerHtml = page.ToString();
                    result.Append(link);
                }
                page++;
            }
            page--; // т.к. будет больше на 1, чем нужно далее

            // Если добавленных ссылок меньше чем кол-во страниц
            if (page < pagingInfo.TotalPages)
            {
                // Указываем на разрыв между текущей страницой и первой страницой, если он сущ-ет
                if (pagingInfo.CurrentPage - 5 >= 1
                    && !(pagingInfo.CurrentPage == pagingInfo.TotalPages && pagingInfo.CurrentPage - 5 == 1))
                {
                    result.Append(" ... ");
                }

                // Если ссылка на след. после текущей страницы еще не была добавлена
                // и текущая страница < max - 1 страницы
                if (page < pagingInfo.CurrentPage + 1 && pagingInfo.CurrentPage < pagingInfo.TotalPages - 1)
                {
                    int i;

                    // Добавим ссылки на текущую, предыд. и след. страницу
                    for (i = pagingInfo.CurrentPage - 1; i <= pagingInfo.TotalPages && i <= pagingInfo.CurrentPage + 1; i++)
                    {
                        if (i <= page)
                        {
                            continue;
                        }
                        else if (i == pagingInfo.CurrentPage)
                        {
                            result.Append(i);
                        }
                        else
                        {
                            TagBuilder link = new TagBuilder("a");
                            link.MergeAttribute("href", pageUrl(i));
                            link.InnerHtml = i.ToString();
                            result.Append(link);
                        }
                    }
                    page = --i; // Для корректного отображения посл. 3 страниц
                }
                
                // Указываем на разрыв между текущей страницой и посл. страницой, если он сущ-ет
                if (pagingInfo.CurrentPage + 5 <= pagingInfo.TotalPages
                    && !(pagingInfo.CurrentPage == 1 && pagingInfo.CurrentPage + 5 == pagingInfo.TotalPages))
                {
                    result.Append(" ... ");
                }

                // Добавим ссылки на посл. 3 страницы
                for (int i = pagingInfo.TotalPages - 2; i <= pagingInfo.TotalPages; i++)
                {
                    if (i <= page)
                    {
                        continue;
                    }
                    else if (i == pagingInfo.CurrentPage)
                    {
                        result.Append(i);
                    }
                    else
                    {
                        TagBuilder link = new TagBuilder("a");
                        link.MergeAttribute("href", pageUrl(i));
                        link.InnerHtml = i.ToString();
                        result.Append(link);
                    }
                }
            }

            // Добавим ссылку на след. страницу
            if (pagingInfo.CurrentPage < pagingInfo.TotalPages)
            {
                TagBuilder link = new TagBuilder("a");
                link.MergeAttribute("href", pageUrl(pagingInfo.CurrentPage + 1));
                link.InnerHtml = " След.";
                result.Append(link);
            }

            return new MvcHtmlString(result.ToString());
        }
    }
}