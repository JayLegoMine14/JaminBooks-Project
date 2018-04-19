using JaminBooks.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace JaminBooks.Tools
{
    public class Receipt
    {
        public static void SendReceipt(Order order)
        {
            var fromAddress = new MailAddress(Authentication.Email, Authentication.Name);
            User u = order.Card.User;
            var toAddress = new MailAddress(u.Email, u.FirstName + " " + u.LastName);
            string subject = "Order Receipt " + DateTime.Now.ToString("MM/dd/yy") + " | Jamin' Books";

            LinkedResource res = new LinkedResource("wwwroot/images/slogo.png");
            res.ContentId = Guid.NewGuid().ToString();

            string body = @"
                <div style=""background-color:#fff;margin:0 auto 0 auto;padding:30px 0 30px 0;color:#1d2227;font-weight:400;font-size:13px;line-height:20px;font-family:'Helvetica Neue',Arial,sans-serif;text-align:left;"">
	                <center>
	                  <table style=""width:550px;text-align:center"">
		                <tbody>
		                  <tr>
			                <td colspan=""2"" style=""padding:30px 0;"">
			                  <img src='cid:" + res.ContentId + @"'/>
			                  <p style=""color:#1d2227;line-height:35px;font-size:25px;margin:12px 10px 20px 10px;font-weight:400;"">Jamin' Books Order Receipt</p>
			                  <div>
				                <div>
					                <table style=""width:550px;text-align:center"">
						                <tr>
							                <td style=""vertical-align:top"">
								                <h4 style=""color:#1d2227;font-weight:bold;"">Card</h4>
								                <div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">****-****-****-" + order.Card.LastFourDigits + @"</div>
								                <div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Card.ExpMonth + @"/" + order.Card.ExpYear + @"</div>
								                <div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Card.Name + @"</div>
							                </td>
							                <td style=""vertical-align:top"">
								                <h4 style=""color:#1d2227;font-weight:bold"">Billing Address</h4>
								                <div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Card.Address.Line1 + @"</div>
								                " + (order.Card.Address.Line2 != null ? @"<div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Card.Address.Line2 + @"</div>" : "") + @"
								                <div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Card.Address.City + @", " + (order.Card.Address.State != null ? order.Card.Address.State : order.Card.Address.Country) + @" " + order.Card.Address.ZIP + @"</div>
							                </td>
							                <td style=""vertical-align:top"">
                                               <h4 style=""color:#1d2227;font-weight:bold"">Billing Address</h4>
								               <div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Address.Line1 + @"</div>
								                " + (order.Address.Line2 != null ? @"<div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Address.Line2 + @"</div>" : "") + @"
								                <div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Address.City + @", " + (order.Address.State != null ? order.Address.State : order.Address.Country) + @" " + order.Address.ZIP + @"</div>
							                </td>
						                </tr>
					                </table>
				                </div>
				                <hr/>
				                <div style=""margin: 20px 0px"">
					                <span style=""font-size:30px;font-weight:bold;margin-right:20px"">Order Total:</span> <span style=""font-size:22px;color:#1d2227;font-weight:400;"">$" + order.Total.ToString("0.00") + @"</span>
				                </div>
				                <hr/>
				                <div>
					                <table style=""width:500px;text-align:center"">
                                        <tr style=""color:#1d2227;font-weight:400;"">
							                <th><div style=""margin-top:15px"">Title</div></th> <th>Quantity</th> <th>Price</th>
						                </tr>";

            foreach (KeyValuePair<Book, dynamic> book in order.Books)
            {
                body += @"
                                            <tr style=""color:#1d2227;font-weight:400;"">
							                    <td><div style=""margin-top:15px"">" + book.Key.Title + @"</div></td> <td>" + book.Value.Quantity + @"</td> <td>$" + book.Value.Price.ToString("0.00") + @"</td>
						                    </tr>";
            }

            if (order.PercentDiscount > 0)
            {
                body += @"
                                            <tr style=""color:#1d2227;font-weight:400;"">
							                    <td colspan=2 style=""font-weight:bold""><div style=""margin-top:20px;"">Discount:</div></td> <td>" + order.PercentDiscount + @"%</td>
						                    </tr>";
            }
            body += @"</table>
				                </div>
			                  </div>
			                </td>
		                  </tr>
		                </tbody>
	                  </table>
	                </center>
                  </div>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, Authentication.Password)
            };

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                IsBodyHtml = true
            })
            {
                message.AlternateViews.Add(alternateView);
                smtp.Send(message);
            }
        }

        public static void SendRefundReceipt(Order order)
        {
            var fromAddress = new MailAddress(Authentication.Email, Authentication.Name);
            User u = order.Card.User;
            var toAddress = new MailAddress(u.Email, u.FirstName + " " + u.LastName);
            string subject = "Refund Receipt " + DateTime.Now.ToString("MM/dd/yy") + " | Jamin' Books";

            LinkedResource res = new LinkedResource("wwwroot/images/slogo.png");
            res.ContentId = Guid.NewGuid().ToString();

            string body = @"
                <div style=""background-color:#fff;margin:0 auto 0 auto;padding:30px 0 30px 0;color:#1d2227;font-weight:400;font-size:13px;line-height:20px;font-family:'Helvetica Neue',Arial,sans-serif;text-align:left;"">
	                <center>
	                  <table style=""width:550px;text-align:center"">
		                <tbody>
		                  <tr>
			                <td colspan=""2"" style=""padding:30px 0;"">
			                  <img src='cid:" + res.ContentId + @"'/>
			                  <p style=""color:#1d2227;line-height:35px;font-size:25px;margin:12px 10px 20px 10px;font-weight:400;"">Jamin' Books Refund Receipt</p>
			                  <div>
				                <div>
					                <table style=""width:550px;text-align:center"">
						                <tr>
							                <td style=""vertical-align:top"">
								                <h4 style=""color:#1d2227;font-weight:bold;"">Card</h4>
								                <div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">****-****-****-" + order.Card.LastFourDigits + @"</div>
								                <div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Card.ExpMonth + @"/" + order.Card.ExpYear + @"</div>
								                <div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Card.Name + @"</div>
							                </td>
							                <td style=""vertical-align:top"">
								                <h4 style=""color:#1d2227;font-weight:bold"">Billing Address</h4>
								                <div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Card.Address.Line1 + @"</div>
								                " + (order.Card.Address.Line2 != null ? @"<div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Card.Address.Line2 + @"</div>" : "") + @"
								                <div style=""margin-bottom:5px;color:#1d2227;font-weight:400;"">" + order.Card.Address.City + @", " + (order.Card.Address.State != null ? order.Card.Address.State : order.Card.Address.Country) + @" " + order.Card.Address.ZIP + @"</div>
							                </td>
						                </tr>
					                </table>
				                </div>
				                <hr/>
				                <div style=""margin: 20px 0px"">
					                <span style=""font-size:30px;font-weight:bold;margin-right:20px"">Refund Total:</span> <span style=""font-size:22px;color:#1d2227;font-weight:400;"">$" + order.Total.ToString("0.00") + @"</span>
				                </div>
				                <hr/>
				                <div>
					                <table style=""width:500px;text-align:center"">
                                        <tr style=""color:#1d2227;font-weight:400;"">
							                <th><div style=""margin-top:15px"">Title</div></th> <th>Quantity</th> <th>Price</th>
						                </tr>";

            foreach (KeyValuePair<Book, dynamic> book in order.Books)
            {
                body += @"
                                            <tr style=""color:#1d2227;font-weight:400;"">
							                    <td><div style=""margin-top:15px"">" + book.Key.Title + @"</div></td> <td>" + book.Value.Quantity + @"</td> <td>$" + book.Value.Price.ToString("0.00") + @"</td>
						                    </tr>";
            }

            if (order.PercentDiscount > 0)
            {
                body += @"
                                            <tr style=""color:#1d2227;font-weight:400;"">
							                    <td colspan=2 style=""font-weight:bold""><div style=""margin-top:20px;"">Discount:</div></td> <td>" + order.PercentDiscount + @"%</td>
						                    </tr>";
            }
            body += @"</table>
				                </div>
			                  </div>
			                </td>
		                  </tr>
		                </tbody>
	                  </table>
	                </center>
                  </div>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, Authentication.Password)
            };

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                IsBodyHtml = true
            })
            {
                message.AlternateViews.Add(alternateView);
                smtp.Send(message);
            }
        }
    }
}
