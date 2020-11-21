using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Challenge.Core.DTO;
using Challenge.Core.DTO.Mappers;
using Challenge.Data.Repository.Interfaces;
using Challenge.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Challenge.API.Controllers
{
    [Route("api/payments/")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private const string InvalidLoginMessage = "Invalid login data, please login again";

        private readonly IPaymentTransactionRepository repository;
        private readonly IApprovePaymentService service;
        private readonly ILogger<PaymentController> logger;

        public PaymentController(
            IPaymentTransactionRepository repository,
            IApprovePaymentService service,
            ILogger<PaymentController> logger)
        {
            this.repository = repository;
            this.service = service;
            this.logger = logger;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(PaymentResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(PaymentDTORequest paymentDTO)
        {
            var userData = GetUserId();

            var merchantId = default(Guid);

            if (string.IsNullOrWhiteSpace(userData) ||
                !Guid.TryParse(userData, out merchantId))
            {
                return BadRequest(InvalidLoginMessage);
            }

            paymentDTO.MerchantId = merchantId;

            var validation = service.ValidateCard(
                paymentDTO.CardNumber,
                paymentDTO.Expire,
                paymentDTO.CVV,
                paymentDTO.OwernName);

            if (!string.IsNullOrWhiteSpace(validation))
            {
                return BadRequest(validation);
            }

            var payment = paymentDTO.ToDomain();

            var paymentResponse = service.ApprovePayment(payment);

            return new JsonResult(new PaymentResponseDTO
            {
                TransactionId = paymentResponse.Id,
                Status = paymentResponse.Status.StatusDescription
            });
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(List<PaymentResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            // Read merchantId from token
            var userData = GetUserId();

            var merchantId = default(Guid);

            if (string.IsNullOrWhiteSpace(userData) ||
                !Guid.TryParse(userData, out merchantId))
            {
                return BadRequest(InvalidLoginMessage);
            }

            var payments = await repository.FindByMerchant(merchantId);

            // Map
            var paymentsDTO = payments
                .Select(p => p.ToDTO())
                .ToList();

            return new JsonResult(paymentsDTO);
        }

        [Authorize]
        [HttpGet("{transactionId}")]
        [ProducesResponseType(typeof(PaymentResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid transactionId)
        {
            var userData = GetUserId();

            var merchantId = default(Guid);

            if (string.IsNullOrWhiteSpace(userData) ||
                !Guid.TryParse(userData, out merchantId))
            {
                return BadRequest(InvalidLoginMessage);
            }

            var payment = await repository.FindById(transactionId);

            if ((payment == null) ||
                (payment != null &&
                payment.Merchant.Id != merchantId))
            {
                return NotFound();
            }

            // Map
            var result = payment.ToDTO();

            return new JsonResult(result);
        }

        private string GetUserId()
        {
            var userData = User.Claims
               .Where(a => a.Type == ClaimTypes.UserData)
               .FirstOrDefault()
               .Value;

            return userData;
        }
    }
}