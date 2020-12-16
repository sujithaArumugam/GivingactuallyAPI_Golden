using firstWebAPI.DataLayer;
using GivingActuallyAPI.Models;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web;
using static GivingActuallyAPI.Models.Helper;

namespace firstWebAPI.Services
{
    public class DonationService
    {
        GivingActuallyEntities Entity = new GivingActuallyEntities();
        public async Task<MiniDonationModel> AddCampaignDonation(CampaignMiniDonation viewModel)
        {
            try
            {
                if (viewModel.id == 0)
                {
                    Tbl_CampaignDonation donation = new Tbl_CampaignDonation();
                    donation.StoryId = viewModel.CampaignId;
                    donation.DonatedBy = viewModel.DonorName;
                    donation.DonatedOn = DateTime.Now;
                    donation.MoneyType = viewModel.DonationMoneyType;
                    donation.DonationAmnt = viewModel.DonationAmnt;
                    donation.PhoneNumber = viewModel.PhoneNumber;
                    donation.Email = viewModel.EMail;
                    donation.isAnanymous = viewModel.isAnanymous;
                    string RazorPayProcessingFeePerc = ConfigurationManager.AppSettings["RazorPayProcessingFeePerc"];

                    string RazorPayGSTPerc = ConfigurationManager.AppSettings["RazorPayGSTPerc"];
                    donation.PayGSTPerc = Convert.ToInt32(Convert.ToDecimal(RazorPayGSTPerc) * 100);
                    donation.PayProcessingFeePerc = Convert.ToInt32(Convert.ToDecimal(RazorPayProcessingFeePerc) * 100);
                    donation.PayProcessingFeeAmnt = viewModel.DonationAmnt * Convert.ToDecimal(RazorPayProcessingFeePerc);

                    donation.PayGSTAmnt = donation.PayProcessingFeeAmnt * Convert.ToDecimal(RazorPayGSTPerc);
                    donation.PaidDOnationAmt = viewModel.DonationAmnt - (donation.PayProcessingFeeAmnt + donation.PayGSTAmnt);
                    donation.Status = true;
                    Entity.Tbl_CampaignDonation.Add(donation);
                    Entity.SaveChanges();
                    var donateId = donation.Id;

                    MiniDonationModel model = new MiniDonationModel();
                    model.Amount = viewModel.DonationAmnt;
                    model.MType = viewModel.DonationMoneyType;
                    model.DonateId = donateId;
                    model.PhNo = viewModel.PhoneNumber;
                    model.Email = viewModel.EMail;
                    model.campaignId = viewModel.CampaignId;

                    model = CreateOrderRazorPay(model);


                    var CurrentDetail = await (from cm in Entity.Tbl_Campaign.AsNoTracking()
                                               join Dn in Entity.Tbl_CampaignDonation.AsNoTracking() on cm.Id equals Dn.StoryId into outerJoinDon
                                               from Dn in outerJoinDon.DefaultIfEmpty()
                                               where cm.Id == viewModel.CampaignId
                                               select new
                                               {
                                                   CampaignId = cm.Id,
                                                   cm.TargetAmount,
                                                   DonationId = Dn != null ? Dn.Id : 0,
                                                   DonatedBy = Dn != null ? Dn.DonatedBy : "",
                                                   DonatedOn = Dn != null ? Dn.DonatedOn : DateTime.Now,
                                                   isAnanymous = Dn != null ? Dn.isAnanymous : true,
                                                   DonationAmnt = Dn != null ? Dn.DonationAmnt : 0,
                                                   isPaid = Dn != null ? Dn.isPaid : false
                                               }).ToListAsync();

                    var DonList = CurrentDetail.Where(x => x.isPaid == true).GroupBy(x => x.CampaignId)
                   .Select(g => new
                   {
                       DonatedBy = g.Key,
                       Total = g.Sum(x => x.DonationAmnt)
                   });

                    var DonorList = CurrentDetail.Where(x => x.isPaid == true)
                         .GroupBy(x => x.DonatedBy)
                         .Select(g => new
                         {
                             DonatedBy = g.Key,
                             Total = g.Sum(x => x.DonationAmnt)
                         });

                    var targetAmount = CurrentDetail.FirstOrDefault()!=null? CurrentDetail.FirstOrDefault().TargetAmount.Value:0;
                    var RaisedAmt = DonList.Any() ? DonList.ToList().FirstOrDefault().Total : 0;
                    decimal difference = targetAmount - RaisedAmt;
                    var RaisedBy = DonorList.Any() ? DonorList.Distinct().ToList().Count() : 0;
                    model.TotalDonorsCount = RaisedBy;
                    model.CanDonate = difference > 0 ? true : false;
                    model.TotalRaisedAmnt = RaisedAmt;
                    model.EligibleTarget = difference > 0 ? difference : 0;

                    return model;
                }
                else
                {
                    var donation = (from S in Entity.Tbl_CampaignDonation where S.Id == viewModel.id select S).FirstOrDefault();

                    if (donation != null)
                    {
                        donation.DonatedBy = viewModel.DonorName;
                        donation.DonatedOn = DateTime.Now;
                        donation.MoneyType = viewModel.DonationMoneyType;
                        donation.DonationAmnt = viewModel.DonationAmnt;
                        donation.PhoneNumber = viewModel.PhoneNumber;
                        donation.Email = viewModel.EMail;
                        donation.isAnanymous = viewModel.isAnanymous;
                        string RazorPayProcessingFeePerc = ConfigurationManager.AppSettings["RazorPayProcessingFeePerc"];

                        string RazorPayGSTPerc = ConfigurationManager.AppSettings["RazorPayGSTPerc"];
                        donation.PayGSTPerc = Convert.ToInt32(Convert.ToDecimal(RazorPayGSTPerc) * 100);
                        donation.PayProcessingFeePerc = Convert.ToInt32(Convert.ToDecimal(RazorPayProcessingFeePerc) * 100);
                        donation.PayProcessingFeeAmnt = viewModel.DonationAmnt * Convert.ToDecimal(RazorPayProcessingFeePerc);

                        donation.PayGSTAmnt = donation.PayProcessingFeeAmnt * Convert.ToDecimal(RazorPayGSTPerc);
                        donation.PaidDOnationAmt = viewModel.DonationAmnt - (donation.PayProcessingFeeAmnt + donation.PayGSTAmnt);
                        donation.Status = true;
                        donation.UpdatedOn = DateTime.UtcNow;
                        Entity.SaveChanges();
                        MiniDonationModel model = new MiniDonationModel();
                        model.Amount = viewModel.DonationAmnt;
                        model.MType = viewModel.DonationMoneyType;
                        model.DonateId = viewModel.id;
                        model.PhNo = viewModel.PhoneNumber;
                        model.Email = viewModel.EMail;
                        model = CreateOrderRazorPay(model);


                        var CurrentDetail = await (from cm in Entity.Tbl_Campaign.AsNoTracking()
                                                   join Dn in Entity.Tbl_CampaignDonation.AsNoTracking() on cm.Id equals Dn.StoryId into outerJoinDon
                                                   from Dn in outerJoinDon.DefaultIfEmpty()
                                                   where cm.Id == viewModel.CampaignId
                                                   select new
                                                   {
                                                       CampaignId = cm.Id,
                                                       cm.TargetAmount,
                                                       DonationId = Dn != null ? Dn.Id : 0,
                                                       DonatedBy = Dn != null ? Dn.DonatedBy : "",
                                                       DonatedOn = Dn != null ? Dn.DonatedOn : DateTime.Now,
                                                       isAnanymous = Dn != null ? Dn.isAnanymous : true,
                                                       DonationAmnt = Dn != null ? Dn.DonationAmnt : 0,
                                                       isPaid = Dn != null ? Dn.isPaid : false
                                                   }).ToListAsync();

                        var DonList = CurrentDetail.Where(x => x.isPaid == true).GroupBy(x => x.CampaignId)
                       .Select(g => new
                       {
                           DonatedBy = g.Key,
                           Total = g.Sum(x => x.DonationAmnt)
                       });

                        var DonorList = CurrentDetail.Where(x => x.isPaid == true)
                             .GroupBy(x => x.DonatedBy)
                             .Select(g => new
                             {
                                 DonatedBy = g.Key,
                                 Total = g.Sum(x => x.DonationAmnt)
                             });

                        var targetAmount = CurrentDetail.FirstOrDefault() != null ? CurrentDetail.FirstOrDefault().TargetAmount.Value : 0;
                        var RaisedAmt = DonList.Any() ? DonList.ToList().FirstOrDefault().Total : 0;
                        decimal difference = targetAmount - RaisedAmt;
                        var RaisedBy = DonorList.Any() ? DonorList.Distinct().ToList().Count() : 0;
                        model.TotalDonorsCount = RaisedBy;
                        model.CanDonate = difference > 0 ? true : false;
                        model.TotalRaisedAmnt = RaisedAmt;
                        model.EligibleTarget = difference > 0 ? difference : 0;
                        return model;
                    }
                    else { return new MiniDonationModel(); }
                }
            }
            catch (Exception ex) { throw ex; }

        }
        public DonationsModel GetDonorscount(int Id)
        {
            try
            {
                DonationsModel DonorsList = new DonationsModel();
                var donations = (from S in Entity.Tbl_CampaignDonation where S.StoryId == Id && S.isPaid == true select S).ToList().OrderByDescending(a => a.DonatedOn);
                if (donations.Any())
                {
                    List<Donation> donationList = new List<Donation>();
                    decimal RaisedAmt = 0;
                    long RaisedBy = 0;
                    decimal RaisedPerc = 0;

                    foreach (var dntion in donations)
                    {
                        RaisedAmt = RaisedAmt + dntion.DonationAmnt;
                        RaisedBy++;

                    }
                    var campagin = (from S in Entity.Tbl_Campaign where S.Id == Id select S).FirstOrDefault();
                    if (campagin != null)
                    {
                        decimal difference = campagin.TargetAmount.Value - RaisedAmt;
                        RaisedPerc = campagin.TargetAmount != null ?
                            (campagin.TargetAmount.Value != 0 ? ((RaisedAmt / campagin.TargetAmount.Value) * 100) : 0) : 0;
                    }
                    DonorsList.AllDonors = new List<Donation>();
                    DonorsList.TotolRaisedAmnt = RaisedAmt;
                    DonorsList.DonorsCount = Convert.ToInt32(RaisedBy);
                    DonorsList.RaisedPercentage = Convert.ToInt32(RaisedPerc);
                    DonorsList.CanEnableDonate = (RaisedAmt >= campagin.TargetAmount.Value) ? false : true;

                }
                return DonorsList;
            }
            catch (Exception ex)
            { throw ex; }
        }
        public MiniDonationModel CreateOrderRazorPay(MiniDonationModel model)
        {
            try
            {
                var typeNmae = model.MType;
                Dictionary<string, object> input = new Dictionary<string, object>();
                input.Add("amount", model.Amount * 100); // this amount should be same as transaction amount
                input.Add("currency", typeNmae);
                input.Add("receipt", model.DonateId.ToString());
                input.Add("payment_capture", 1);

                Dictionary<string, string> notees = new Dictionary<string, string>();
                notees.Add("CampaignId", model.campaignId.ToString()); // this amount should be same as transaction amount

                input.Add("notes", notees);
                string key = ConfigurationManager.AppSettings["RPapikey"];
                string secret = ConfigurationManager.AppSettings["RPSecretkey"];
                //string key = "rzp_test_5iPslGRlz5M0ss";
                //string secret = "qEsCxVcyaRoSAatpBaVZMBdp";
                ////string key = "rzp_live_S9v0s4ePfuxE1p";
                ////string secret = "KJylHsBzeIlVeF336TPxjk6w";
                System.Net.ServicePointManager.SecurityProtocol = System.Net.ServicePointManager.SecurityProtocol | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
                RazorpayClient client = new RazorpayClient(key, secret);
                Razorpay.Api.Order order = client.Order.Create(input);
                var orderId = order["id"].ToString();
                model.orderId = orderId;
                model.RazorPayKey = key;
                UpdateCampaignDonationorder(model.DonateId, orderId, "", "");
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateCampaignDonationorder(int id, string orderId, string payementID, string signature)
        {
            try
            {
                var donation = (from S in Entity.Tbl_CampaignDonation where S.Id == id select S).FirstOrDefault();
                if (donation != null)
                {
                    donation.razorpay_order_id = orderId;
                    donation.razorpay_payment_id = payementID;
                    donation.razorpay_signature = signature;
                    //if (string.IsNullOrEmpty(payementID))
                    //    donation.isPaid = true;
                    donation.Status = true;
                    donation.UpdatedOn = DateTime.UtcNow;
                    Entity.SaveChanges();
                    return donation.Id;
                }
                else { return 0; }

            }
            catch (Exception ex) { throw ex; }
        }

        public int UpdateCampaignDonationSuccess(string razorpay_order_id, string razorpay_payment_id, string razorpay_signature)
        {
            try
            {
                var donation = (from S in Entity.Tbl_CampaignDonation where S.razorpay_order_id == razorpay_order_id select S).FirstOrDefault();

                if (donation != null)
                {
                    donation.razorpay_order_id = razorpay_order_id;
                    donation.razorpay_payment_id = razorpay_payment_id;
                    donation.razorpay_signature = razorpay_signature;
                    if (!string.IsNullOrEmpty(razorpay_payment_id))
                    {
                        CapturePayment(razorpay_payment_id, donation.DonationAmnt * 100, "INR");
                        donation.isPaid = true;
                    }
                    donation.Status = true;
                    donation.UpdatedOn = DateTime.UtcNow;
                    Entity.SaveChanges();
                    return donation.StoryId;
                }
                else { return 0; }


               
            }
            catch (Exception ex) { throw ex; }

        }

        public int CapturePayment( string razorpay_payment_id,decimal Amount, string Moneytype)
        {
            try
            {
             
                string key = ConfigurationManager.AppSettings["RPapikey"];
                string secret = ConfigurationManager.AppSettings["RPSecretkey"];
                System.Net.ServicePointManager.SecurityProtocol = System.Net.ServicePointManager.SecurityProtocol | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
                RazorpayClient client = new RazorpayClient(key, secret);

                Payment payment = client.Payment.Fetch(razorpay_payment_id);

                Dictionary <string, object> options = new Dictionary<string, object>();
                options.Add("amount", Amount);
                options.Add("currency", Moneytype);
                Payment paymentCaptured = payment.Capture(options);
                return 1;
            }
            catch (Exception ex) { throw ex; }

        }
    }
}