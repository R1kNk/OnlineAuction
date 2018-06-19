using SOOS_Auction.AuctionDatabase.Models;
using SOOS_Auction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOOS_Auction.AuctionDatabase.Shared
{
    public class AuctionDatabaseInitializer : DropCreateIfDatabaseEmpty
    {
        protected override void Seed(AuctionContext context)
        {
            //List<Lot> lots = new List<Lot>() { new Lot { Name = "Мультиварка Bosch", State = "б/у", StartDate = DateTime.Now, FinishDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 2) } };
            //for (int i = 0; i < lots.Count; i++)
            //{
            //    context.Lots.Add(lots[i]);
            //}
            //context.SaveChanges();
            List<Section> sections = new List<Section>() { new Section() { Name = "Телефоны. Смартфоны." }, new Section() { Name = "Ноутбуки. Компьютеры. Apple. Оргтехника" }, new Section() { Name = "Фото. Аудио. Видео. Приставки" }, new Section() { Name = "Авто. Мото." }, new Section() { Name = "Автозапчасти и Автоаксессуары" }, new Section() { Name = "Дом. Дача" }, new Section() { Name = "Коллекционирование" }, new Section() { Name = "Одежда. Обувь. Аксессуары." } };
            context.Sections.AddRange(sections);
            context.SaveChanges();
            int telephoneSectionId = context.Sections.Where(p => p.Name.Contains("Телефоны")).Select(p=>p.SectionId).Single();
            List<Category> telephoneCategories = new List<Category>()
            {
                new Category() { Name = "Мобильные телефоны", SectionId = telephoneSectionId },
                new Category() { Name = "Мобильные телефоны: Аксессуары и запчасти", SectionId = telephoneSectionId },
                new Category() { Name = "Радиотелефоны, DECT  и Проводные телефоны", SectionId = telephoneSectionId },
                new Category() { Name = "Другое", SectionId = telephoneSectionId },
            };
            context.Categories.AddRange(telephoneCategories);
            context.SaveChanges();
            
            int notebooksSectionId = context.Sections.Where(p => p.Name.Contains("Ноутбуки")).Select(p => p.SectionId).Single();
            List<Category> notebookCategories = new List<Category>()
            {
                new Category() { Name = "Ноутбуки", SectionId = notebooksSectionId },
                new Category() { Name = "Планшеты и электронные книги", SectionId = notebooksSectionId },
                new Category() { Name = "Apple. Mac, iPod, iPhone, iPad", SectionId = notebooksSectionId },
                new Category() { Name = "Apple: запчасти и аксессуары", SectionId = notebooksSectionId },
                new Category() { Name = "Материнские платы. Процессоры. Оперативная память", SectionId = notebooksSectionId },
                new Category() { Name = "Видеокарты", SectionId = notebooksSectionId },
                new Category() { Name = "Корпуса. Блоки питания. ИБП. Системы охлаждения. Моддинг", SectionId = notebooksSectionId },
                new Category() { Name = "Мониторы. Проекторы", SectionId = notebooksSectionId },
                new Category() { Name = "Акустика. Клавиатуры. Мыши. Аксессуары.", SectionId = notebooksSectionId },
                new Category() { Name = "Сетевое оборудование. Модемы. WI-FI", SectionId = notebooksSectionId },
                new Category() { Name = "Принтеры. МФУ. Офисная техника", SectionId = notebooksSectionId },
                new Category() { Name = "Радиотехника и электроника", SectionId = notebooksSectionId },
                new Category() { Name = "Другое", SectionId = notebooksSectionId },
            };
            context.Categories.AddRange(notebookCategories);
            context.SaveChanges();

            int photoSectionId = context.Sections.Where(p => p.Name.Contains("Фото")).Select(p => p.SectionId).Single();
            List<Category> photoCategories = new List<Category>()
            {
                new Category() { Name = "Фотоаппараты", SectionId = photoSectionId },
                new Category() { Name = "Фотоаксессуары. Объективы", SectionId = photoSectionId },
                new Category() { Name = "Видеокамеры. Аксессуары", SectionId = photoSectionId },
                new Category() { Name = "MP-3 плееры и Портативная аудио/видео-техника. Наушники.", SectionId = photoSectionId },
                new Category() { Name = "Спутниковое телевидение", SectionId = photoSectionId },
                new Category() { Name = "Телевизоры", SectionId = photoSectionId },
                new Category() { Name = "Игровые приставки", SectionId = photoSectionId },
                new Category() { Name = "Другое", SectionId = photoSectionId },
            };
            context.Categories.AddRange(photoCategories);
            context.SaveChanges();

            int carsSectionId = context.Sections.Where(p => p.Name.Contains("Авто. Мото")).Select(p => p.SectionId).Single();
            List<Category> carsCategories = new List<Category>()
            {
                new Category() { Name = "Грузовые автомобили и прицепы", SectionId = carsSectionId },
                new Category() { Name = "Спец. техника. Трактора", SectionId = carsSectionId },
                new Category() { Name = "Автобусы", SectionId = carsSectionId },
                new Category() { Name = "Водный транспорт", SectionId = carsSectionId },
                new Category() { Name = "Мотозапчасти. мотоаксессуары", SectionId = carsSectionId },
                new Category() { Name = "Другое", SectionId = carsSectionId },
            };
            context.Categories.AddRange(carsCategories);
            context.SaveChanges();

            int carGearSectionId = context.Sections.Where(p => p.Name.Contains("Автозапчасти")).Select(p => p.SectionId).Single();
            List<Category> carsGearCategories = new List<Category>()
            {
                new Category() { Name = "Расходники", SectionId = carGearSectionId },
                new Category() { Name = "Двигатели. Система пуска и зарядки. Система зажигания", SectionId = carGearSectionId },
                new Category() { Name = "Система охлаждения, отопления, вентиляции", SectionId = carGearSectionId },
                new Category() { Name = "Топливная систем,а. Система спуска", SectionId = carGearSectionId },
                new Category() { Name = "Сцепление. Коробка передач. Привод", SectionId = carGearSectionId },
                new Category() { Name = "Подвеска. Рулевое управление", SectionId = carGearSectionId },
                new Category() { Name = "Тормозная система.", SectionId = carGearSectionId },
                new Category() { Name = "Автомобильная оптика. Тюнинг. Автосигнализация", SectionId = carGearSectionId },
                new Category() { Name = "Шины и диски", SectionId = carGearSectionId },
                new Category() { Name = "Аккумуляторы", SectionId = carGearSectionId },
                new Category() { Name = "GPS навигаторы", SectionId = carGearSectionId },
                new Category() { Name = "Видеорегистраторы", SectionId = carGearSectionId },
                new Category() { Name = "Другое", SectionId = carGearSectionId },
            };
            context.Categories.AddRange(carsGearCategories);
            context.SaveChanges();

            int houseSectionId = context.Sections.Where(p => p.Name.Contains("Дом.")).Select(p => p.SectionId).Single();
            List<Category> houseCategories = new List<Category>()
            {
                new Category() { Name = "Шкафы. Комоды. Горки. Секции. Полки", SectionId = houseSectionId },
                new Category() { Name = "Диваны. Кресла. Мягкая мебель.", SectionId = houseSectionId },
                new Category() { Name = "Столы. Стулья. Тумбы", SectionId = houseSectionId },
                new Category() { Name = "Кровати. Матрасы. Мебель для спальни", SectionId = houseSectionId },
                new Category() { Name = "Кухни и кухонная мебель", SectionId = houseSectionId },
                new Category() { Name = "Мебель для детской комнаты", SectionId = houseSectionId },
                new Category() { Name = "Мебель для ванной", SectionId = houseSectionId },
                new Category() { Name = "Офисная мебель", SectionId = houseSectionId },
                new Category() { Name = "Элементы интерьера. Дизайн", SectionId = houseSectionId },
                new Category() { Name = "Посуда и кухонные принадлежности", SectionId = houseSectionId },
                new Category() { Name = "Бытовая техника", SectionId = houseSectionId },
                new Category() { Name = "Растения. Дача. Сад и огород. Ландшафтный дизайн", SectionId = houseSectionId },
                new Category() { Name = "Газонокосилки. Триммеры. Кусторезы", SectionId = houseSectionId },
                new Category() { Name = "Другое", SectionId = houseSectionId },
            };
            context.Categories.AddRange(houseCategories);
            context.SaveChanges();
            
            int collectingSectionId = context.Sections.Where(p => p.Name.Contains("Коллекционирование")).Select(p => p.SectionId).Single();
            List<Category> collectingCategories = new List<Category>()
            {
                new Category() { Name = "Медали. Жетоны. Значки", SectionId = collectingSectionId },
                new Category() { Name = "Банкноты. Ценные бумаги", SectionId = collectingSectionId },
                new Category() { Name = "Военные вещи", SectionId = collectingSectionId },
                new Category() { Name = "Марки", SectionId = collectingSectionId },
                new Category() { Name = "Модели. Фигурки", SectionId = collectingSectionId },
                new Category() { Name = "Аксессуары", SectionId = collectingSectionId },
                new Category() { Name = "Другое", SectionId = collectingSectionId },
            };
            context.Categories.AddRange(collectingCategories);
            context.SaveChanges();

            int clothesSectionId = context.Sections.Where(p => p.Name.Contains("Одежда")).Select(p => p.SectionId).Single();
            List<Category> clothesCategories = new List<Category>()
            {
                new Category() { Name = "Женская одежда", SectionId = clothesSectionId },
                new Category() { Name = "Женская обувь", SectionId = clothesSectionId },
                new Category() { Name = "Мужская одежда", SectionId = clothesSectionId },
                new Category() { Name = "Мужская обувь", SectionId = clothesSectionId },
                new Category() { Name = "Детская одежда", SectionId = clothesSectionId },
                new Category() { Name = "Детская обувь", SectionId = clothesSectionId },
                new Category() { Name = "Головные уборы", SectionId = clothesSectionId },
                new Category() { Name = "Бижутерия", SectionId = clothesSectionId },
                new Category() { Name = "Наручные часы", SectionId = clothesSectionId },
                new Category() { Name = "Аксессуары", SectionId = clothesSectionId },
                new Category() { Name = "Старинная одежда. Винтаж", SectionId = clothesSectionId },
                new Category() { Name = "Другое", SectionId = clothesSectionId },

            };
            context.Categories.AddRange(clothesCategories);
            context.SaveChanges();
        }
    }
}