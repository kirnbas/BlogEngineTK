using BlogEngineTK.WebUI.Infrastructure.ModelBinders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;

namespace BlogEngineTK.WebUI.Infrastructure.HttpHandlers
{
    /// <summary>
    /// Класс генерирующий капчу для детектирования робота
    /// </summary>
    public class Captcha : IHttpHandler, IRequiresSessionState
    {
        Random random = new Random();

        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// Генерирует капчу и сохраняет её код в SessionDataStore для сравнения с вводимым
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            string code = GetRandomText();

            // Сохранение сгенерированного кода			
            SessionStorage.Current.CaptchaCode = code;

            // Создание картинки на основе сгенерированного кода

            Bitmap map = new Bitmap(140, 40);

            Graphics g = Graphics.FromImage(map);
            g.Clear(Color.White);
            g.DrawRectangle(new Pen(Brushes.Black), 0, 0, 139, 39);

            int dx = 0;

            for (int i = 0; i < code.Length; i++)
            {
                g.DrawString(code[i].ToString(),
                            new Font("Verdana",
                            random.Next(10, 14)),
                            Brushes.Blue,
                            new Point(10 + dx, 10));
                dx += 20;
            }

            // Снижение возможности парсинга изображение
            DrawRandomLines(g);

            // Сохранение изображения в потоке ответа
            map.Save(context.Response.OutputStream, ImageFormat.Gif);

            g.Dispose();
            map.Dispose();

            context.Response.End();
        }

        /// <summary>
        /// Возвращает случайный текст в зависимости от заданной настройки CaptchaLevel в web.config
        /// </summary>
        /// <returns></returns>
        private string GetRandomText()
        {
            StringBuilder code = new StringBuilder();

            switch (WebConfigurationManager.AppSettings.Get("CaptchaLevel"))
            {
                case "1":
                    // 1 уровень сложности
                    for (int i = 0; i < 6; i++)
                    {
                        switch (random.Next(2))
                        {
                            case 0:
                                // только заглавные буквы для удобочитаемости, 
                                code.Append((char)random.Next(65, 90));
                                break;
                            default:
                                // исключаем 0 для той же удобочитаемости
                                code.Append(random.Next(1, 9));
                                break;
                        }
                    }
                    break;
                case "2":
                    // 2 уровень сложности кода в капче (разный регистр букв)
                    for (int i = 0; i < 6; i++)
                    {
                        switch (random.Next(2))
                        {
                            case 0:
                                code.Append(
                                    (random.Next(2) == 1) ?
                                    (char)random.Next(65, 90) :
                                    (char)random.Next(97, 122)
                                    );
                                break;
                            default:
                                code.Append(random.Next(0, 9));
                                break;
                        }
                    }
                    break;
                default:
                    // 3 уровень сложности кода в капче (добавляются специальные символы)
                    for (int i = 0; i < 6; i++)
                    {
                        switch (random.Next(2))
                        {
                            case 0:
                                code.Append(
                                    (random.Next(2) == 1) ?
                                    (char)random.Next(65, 90) :
                                    (char)random.Next(97, 122)
                                    );
                                break;
                            default:
                                if (random.Next(2) == 1)
                                    code.Append(random.Next(0, 9));
                                else
                                    code.Append((char)random.Next(33, 47));
                                break;
                        }
                    }
                    break;
            }

            return code.ToString();
        }

        /// <summary>
        /// Рисование случайных по длине и координатам линий для затруднения парсинга капчи
        /// </summary>
        /// <param name="g"></param>
        private void DrawRandomLines(Graphics g)
        {
            SolidBrush red = new SolidBrush(Color.Red);

            Func<Point[]> getRandomPoints = () =>
            {
                return new Point[]
				{ 
					new Point(random.Next(5, 140), 
					random.Next(5, 40)), 
                    new Point(random.Next(5, 140), 
					random.Next(5, 40)) 
				};
            };

            for (int i = 0; i < 7; i++)
            {
                g.DrawLines(new Pen(red, 1), getRandomPoints());
            }
        }
    }
}