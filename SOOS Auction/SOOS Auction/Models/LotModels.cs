using SOOS_Auction.AuctionDatabase.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SOOS_Auction.Models
{
    public class LotCreate
    {
        [Required]
        [Display(Name="Краткая информация о лоте")]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [Display(Name="Стартовая цена")]
        [Range(0.1,Double.MaxValue, ErrorMessage ="Стартовая цена должна быть больше 0")]
        public double MinimalPrice { get; set; }

        [Required]
        [Display(Name = "Минимальный шаг ставки")]
        [Range(0.1,Double.MaxValue, ErrorMessage ="Минимальный шаг должен быть больше 0")]
        public double MinimalStep { get; set; }

        [Required]
        [MaxLength(300)]
        [Display(Name ="Подробное описание вашего лота")]
        public string Description { get; set; }

        [Required]
        [Display(Name ="Сколько дней будет длиться аукцион?")]
        [Range(1, 90, ErrorMessage = "Минимальная длительность аукциона - 1 день, максимальная - 90 дней")]
        public int DaysDuration { get; set; }

        [Required]
        [Display(Name = "Раздел:")]
        public int SectionId { get; set; }

        [Required]
        [Display(Name ="Категория:")]
        public int CategoryId { get; set; }

        //Receiving
        [Required]
        [Display(Name="Местоположение:")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Отправка почтой")]
        public bool ByPost { get; set; }

        [Required]
        [Display(Name = "Передача лично")]
        public bool DeliveryInPerson { get; set; }

        [Required]
        [Display(Name = "Я согласен отпрвить почтой в другую страну")]
        public bool ByPostToAnotherCountry { get; set; }

        [Required]
        [Display(Name = "Невозможен возврат товара после поупки")]
        public bool ReturnAfterBuyingIsForbidden { get; set; }
        //Payment

        [Required]
        [Display(Name = "Наличными при встрече")]
        public bool Cash { get; set; }
        [Required]
        [Display(Name = "Безналичный расчет")]
        public bool NonCash { get; set; }
        [Required]
        [Display(Name = "Полная предоплата до отправки лота по почте")]
        public bool FullPrepaymentPostSending { get; set; }

        [Display(Name="Подробнее об оплате и доставке:")]
        public string AdditionalInformation { get; set; }
    }

    public class LotDetails
    {
        [Required]
        public int LotId { get; set; } //ok
        [Required]
        [Display(Name = "Краткая информация о лоте")]
        [MaxLength(30)]
        public string Name { get; set; } //ok

        [Required]
        public string State { get; set; }

        [Required]
        [Display(Name = "Стартовая цена")]
        [Range(0.1, Double.MaxValue, ErrorMessage = "Стартовая цена должна быть больше 0")]
        public double MinimalPrice { get; set; } //ok

        [Required]
        [Display(Name = "Минимальный шаг ставки")]
        public double MinimalStep { get; set; } //ok

        [Required]
        [Range(0.1, Double.MaxValue)]
        public double CurrentPrice { get; set; } //ok

        [Required]
        [MaxLength(300)]
        [Display(Name = "Подробное описание вашего лота")]
        public string Description { get; set; } //ok

        [Required]
        [Display(Name = "Сколько дней будет длиться аукцион?")]
        [Range(1, 90, ErrorMessage = "Минимальная длительность аукциона - 1 день, максимальная - 90 дней")]
        public int DaysDuration { get; set; } //ok

        public DateTime StartDate { get; set; }//ok

        public DateTime FinishDate { get; set; }//ok


        [Required]
        [Display(Name = "Раздел:")]
        public string SectionName { get; set; } //ok

        [Required]
        [Display(Name = "Категория:")]
        public string CategoryName { get; set; } //ok

        //Receiving
        [Required]
        [Display(Name = "Местоположение:")]
        public string Location { get; set; } //ok

        [Required]
        [Display(Name = "Отправка почтой")]
        public bool ByPost { get; set; } //ok

        [Required]
        [Display(Name = "Передача лично")]
        public bool DeliveryInPerson { get; set; } //ok

        [Required]
        [Display(Name = "Я согласен отпрвить почтой в другую страну")]
        public bool ByPostToAnotherCountry { get; set; } //ok

        [Required]
        [Display(Name = "Невозможен возврат товара после поупки")]
        public bool ReturnAfterBuyingIsForbidden { get; set; } //ok
        //Payment

        [Required]
        [Display(Name = "Наличными при встрече")]
        public bool Cash { get; set; } //ok
        [Required]
        [Display(Name = "Безналичный расчет")]
        public bool NonCash { get; set; } //ok
        [Required]
        [Display(Name = "Полная предоплата до отправки лота по почте")]
        public bool FullPrepaymentPostSending { get; set; }

        [Display(Name = "Подробнее об оплате и доставке:")]
        public string PostPaymentAdditionalInformation { get; set; }

        //users

        public string UserId { get; set; } //ok
        public int UserPositiveReviews { get; set; }
        public int UserNegativeReviews { get; set; }
        public string UserLocation { get; set; }
        public string UserAvatarUrl { get; set; }

        public string WinnerId { get; set; } //ok

        public string UserName { get; set; } //ok

        public string WinnerName { get; set; } //ok
        //images
        public List<string> UserImagesID { get; set; }

        public List<Bid> Bids { get; set; }

    }

    public class BidDetails
    {
        public Bid bid { get;set; }
        public string UserName { get; set; }
        public string LotOwnerUserName { get; set; }
        public string CurrentUserName { get; set; }
        public string LotState { get; set; }

    }

    public class LotData:WinnerInfo
    {
        public string newLotPrice { get; set; }
        public string newPlaceHolder { get; set; }
    }

    public class WinnerInfo
    {
        public string WinnerId { get; set; }
        public string WinnerName { get; set; }
        public bool isSuccess { get; set; }


    }
    public class NewBidResult : WinnerInfo
    {
        public NewBidResult()
        {
            bidErrors = new List<string>();
        }

        public List<string> bidErrors { get; set; }
 
    }

}
