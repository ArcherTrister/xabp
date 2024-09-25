// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Payment.WeChatPay;

public static class WeChatPayConsts
{
    /// <summary>
    /// Value: "WeChatPay"
    /// </summary>
    public const string GatewayName = "WeChatPay";

    /// <summary>
    /// Value: "/Payment/WeChat/PrePayment"
    /// </summary>
    public const string PrePaymentUrl = "/Payment/WeChat/PrePayment";

    /// <summary>
    /// Value: "/Payment/WeChat/PostPayment"
    /// </summary>
    public const string PostPaymentUrl = "/Payment/WeChat/PostPayment";

    public static class ParameterNames
    {
        public const string Token = "token";
    }

    public static class OrderStatus
    {
        /// <summary>
        /// The order was created with the specified context.
        /// </summary>
        public const string Created = "CREATED";

        /// <summary>
        /// The order was saved and persisted. The order status continues to be in progress until a capture is made with final_capture = true for all purchase units within the order.
        /// </summary>
        public const string Saved = "SAVED";

        /// <summary>
        /// The customer approved the payment through the WeChat wallet or another form of guest or unbranded payment. For example, a card, bank account, or so on.
        /// </summary>
        public const string Approved = "APPROVED";

        /// <summary>
        /// All purchase units in the order are voided.
        /// </summary>
        public const string Voided = "VOIDED";

        /// <summary>
        /// The payment was authorized or the authorized payment was captured for the order.
        /// </summary>
        public const string Completed = "COMPLETED";

        /// <summary>
        /// The order requires an action from the payer (e.g. 3DS authentication). Redirect the payer to the "rel":"payer-action" HATEOAS link returned as part of the response prior to authorizing or capturing the order.
        /// </summary>
        public const string PlayerActionRequired = "PAYER_ACTION_REQUIRED";
    }
}
