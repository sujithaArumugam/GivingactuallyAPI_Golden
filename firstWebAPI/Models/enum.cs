using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace GivingActuallyAPI.Models
{
    public static class Helper
    {

        public enum RolesEnum
        {
            Admin = 0,
            User = 1
        }

        public enum StoryCategory
        {

            [Display(Name = "Medical & Illness")]
            Medical = 9,
            [Display(Name = "Agriculture")]
            Agriculture = 1,
            [Display(Name = "Animals")]
            Animals = 2,
            [Display(Name = "Annadhanam & Food")]
            Annadhanam = 3,
            [Display(Name = "Charity")]
            Charity = 4,
            [Display(Name = "Education")]
            Education = 5,
            [Display(Name = "Elderly Care")]
            ElderlyCare = 6,
            [Display(Name = "Emergency")]
            Emergency = 7,
            [Display(Name = "Funeral")]
            Funeral = 8,
      //      [Display(Name = "Mental Health")]
        //    MentalHealth = 10,
            [Display(Name = "Nutrition")]
            Nutrition = 11,
            [Display(Name = "Spirituality")]
            Spirituality = 12,
            [Display(Name = "Sports")]
            Sports = 13,
            [Display(Name = "Volunteer")]
            Volunteer = 14,
            //   [Display(Name = "Wedding")]
            // Wedding = 15,
            [Display(Name = "Others")]
            Others = 16,

        }
        public enum MoneyType
        {
            //[Display(Name = "EUR")]
            //EUR = 0,
            [Display(Name = "INR")]
            INR = 1


        }
        public static string DisplayName(this Enum value)
        {
            Type enumType = value.GetType();
            var enumValue = System.Enum.GetName(enumType, value);
            MemberInfo member = enumType.GetMember(enumValue)[0];

            var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            var outString = ((DisplayAttribute)attrs[0]).Name;

            if (((DisplayAttribute)attrs[0]).ResourceType != null)
            {
                outString = ((DisplayAttribute)attrs[0]).GetName();
            }

            return outString;
        }


        public enum BeneficiaryType
        {
            Select = 0,
            [Display(Name = "Myself")]
            Myself = 1,
            [Display(Name = "My Family - Individual")]
            FamilyIndividual = 2,
            [Display(Name = "My Family - Group")]
            FamilyGroup = 3,
            [Display(Name = "My Friends - Individual")]
            FriendIndividual = 4,
            [Display(Name = "My Friends - Group")]
            FriendGroup = 5,
            [Display(Name = "Others")]
            Others =6,
            [Display(Name = "Registered NGO")]
            NGO = 7
        }

        public enum GenderType
        {
            Male = 1,
            Female = 2,
            Others = 3
        }
        public enum ViewType
        {
            New = 1,
            Pending = 2,
            Fraud = 3

        }
        public enum NGOType
        {
            [Display(Name = "Private Sector Companies (sec 8/25)")]
            PrivateSectorCompanies = 1,
            [Display(Name = "Registered Societies(Non- Government)")]
            RegisteredSocieties = 2,
            [Display(Name = "Trust(Non- Government)")]
            Trust = 3,
            [Display(Name = "Other Registered Entities(Non- Government)")]
            OtherRegistered = 4,
            [Display(Name = "Academic Institutions(Private)")]
            Academic = 5
        }
    }
}