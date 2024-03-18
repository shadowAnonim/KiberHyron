using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiberHyron
{
    public class GameModal : IModal
    {
        public string Title => "Создание игры";
        [InputLabel("Название")]
        [ModalTextInput("name", Discord.TextInputStyle.Short, "Название игры")]
        public string Name { get; set; }
        [InputLabel("Описание")]
        [ModalTextInput("description", Discord.TextInputStyle.Paragraph, "Описание игры")]
        [RequiredInput(false)]
        public string Description { get; set; }
        [InputLabel("Ссылка")]
        [ModalTextInput("link", Discord.TextInputStyle.Short, "Ссылка на Roll20")]
        public string Link { get; set; }
        [InputLabel("Цвет")]
        [ModalTextInput("color", Discord.TextInputStyle.Paragraph, "HEX-значение цвета (если вы не заполните это поле, будет выбран случайный цвет)")]
        [RequiredInput(false)]
        public string Color { get; set; }
    }
}
