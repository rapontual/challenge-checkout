using System;
using System.Linq;
using Challenge.Core.Model;

namespace Challenge.Core.DTO.Mappers
{
    public static class DTOMappersExtension
    {
        public static PaymentTransaction ToDomain(this PaymentCreateRequestDTO dto)
        {
            return new PaymentTransaction
            {
                Amout = dto.Amout,
                CardNumber = dto.CardNumber,
                CurrencyId = dto.CurrencyId,
                CVV = dto.CVV,
                Expire = dto.Expire,
                Merchant = new Merchant
                {
                    Id = dto.MerchantId
                },
                OwernName = dto.OwernName
            };
        }

        public static PaymentResponseDTO ToDTO(this PaymentTransaction payment)
        {
            var cardNumber = new String('X', payment.CardNumber.ToString().Length);

            return new PaymentResponseDTO
            {
                Amout = payment.Amout,
                CardNumber = cardNumber,
                Currency = payment.Currency.Name,
                Expire = payment.Expire,
                Id = payment.Id,
                Merchant = payment.Merchant.Name,
                OwernName = payment.OwernName,
                Status = payment.Status.StatusDescription,
                Date = payment.Date
            };
        }
    }
}
