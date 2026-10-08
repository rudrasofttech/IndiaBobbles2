@Code
    ViewData("Title") = "Payment options offered by IndiaBobbles"
End Code

@section meta
    <meta name="description" content="Pay for your India Bobbles order by UPI, credit or debit card, net banking, cash before delivery or cash on delivery (New Delhi / NCR). Secure payments by PayU." />
End Section

<div class="ib-legal ib-ship ib-pay container fullbody">

    <!-- Heading -->
    <div class="hero">
        <h1>Payment Options</h1>
        <p>Pay the way you like. Every online payment is processed securely by PayU.</p>
        <span class="upd">Last updated: 8 October 2026</span>
    </div>

    <!-- Payment methods -->
    <div class="row g-3 mb-4">
        <div class="col-6 col-lg-4"><div class="method"><i class="fa fa-mobile" aria-hidden="true"></i><b>UPI</b><span>Google Pay, PhonePe, Paytm, BHIM and other UPI apps</span></div></div>
        <div class="col-6 col-lg-4"><div class="method"><i class="fa fa-credit-card" aria-hidden="true"></i><b>Credit cards</b><span>Indian and international cards</span></div></div>
        <div class="col-6 col-lg-4"><div class="method"><i class="fa fa-credit-card-alt" aria-hidden="true"></i><b>Debit cards</b><span>Cards issued by Indian banks</span></div></div>
        <div class="col-6 col-lg-4"><div class="method"><i class="fa fa-university" aria-hidden="true"></i><b>Net banking</b><span>Pay directly from your bank account</span></div></div>
        <div class="col-6 col-lg-4"><div class="method"><i class="fa fa-money" aria-hidden="true"></i><b>Cash before delivery</b><span>Pay before we dispatch your order</span></div></div>
        <div class="col-6 col-lg-4"><div class="method"><i class="fa fa-truck" aria-hidden="true"></i><b>Cash on delivery</b><span class="tag-limit">New Delhi / NCR only</span></div></div>
    </div>

    <div class="row g-4">
        <div class="col-lg-3">
            <nav class="toc" aria-label="Contents">
                <h6>Contents</h6>
                <ol>
                    <li><a href="#how">How to pay</a></li>
                    <li><a href="#secure">Is it secure?</a></li>
                    <li><a href="#payu">Who is PayU?</a></li>
                    <li><a href="#intl">International cards</a></li>
                    <li><a href="#cash">Cash payments</a></li>
                    <li><a href="#failed">Payment failed?</a></li>
                    <li><a href="#refunds">Refunds</a></li>
                    <li><a href="#safety">Stay safe</a></li>
                    <li><a href="#contact">Contact us</a></li>
                </ol>
            </nav>
        </div>

        <div class="col-lg-9">
            <div class="doc">

                <h2 id="how" style="margin-top:0;">1. How to pay for your order</h2>
                <ol class="steps">
                    <li><b>Add your bobbleheads to the cart</b> and click <b>Checkout</b>.</li>
                    <li><b>Enter your delivery details.</b></li>
                    <li><b>Choose a payment method.</b> You'll be taken to PayU's secure payment page.</li>
                    <li><b>Complete the payment</b>, approving it in your UPI app or bank's OTP screen if asked. You'll return to India Bobbles and receive an order confirmation email.</li>
                </ol>
                <p>All prices are in <b>Indian Rupees (₹)</b> and include applicable taxes. Shipping is free across India.</p>

                <h2 id="secure">2. Is it secure?</h2>
                <div class="secure">
                    <i class="fa fa-lock" aria-hidden="true"></i>
                    <div>
                        <p class="mb-2">Yes. Online payments are handled by our payment gateway partner, <b>PayU</b>, over an encrypted connection. Your card, UPI and bank details are entered on PayU's secure page.</p>
                        <p class="mb-0"><b>India Bobbles never sees or stores</b> your full card number, CVV, UPI PIN or net banking password.</p>
                    </div>
                </div>

                <h2 id="payu">3. Who is PayU?</h2>
                <p>PayU is one of India's leading online payment gateways. It works with banks and card networks to accept and verify online payments by card, UPI and net banking in real time, providing a secure link between the India Bobbles website, your bank and the card networks. Learn more at <a href="https://payu.in" target="_blank" rel="noopener">payu.in</a>.</p>

                <h2 id="intl">4. International cards</h2>
                <p>We accept cards issued outside India. Your card is charged in <b>Indian Rupees (₹)</b>, and your bank converts the amount into your local currency at its current exchange rate. Your bank may also add a foreign transaction fee. Please note that we currently deliver <b>only to addresses in India</b>.</p>

                <h2 id="cash">5. Cash payments</h2>
                <ul>
                    <li><b>Cash before delivery:</b> choose this at checkout and we'll send you payment details. We dispatch your order once we receive the payment.</li>
                    <li><b>Cash on delivery (COD):</b> available for delivery addresses in <b>New Delhi / NCR only</b>. Please keep the exact amount ready for the courier.</li>
                </ul>

                <h2 id="failed">6. Payment failed, or money deducted but no order?</h2>
                <p>Don't worry, this happens occasionally because of bank or network delays.</p>
                <ul>
                    <li>Check <a href="~/orders">My Orders</a> and your email after a few minutes. Your order may have gone through.</li>
                    <li>If the money left your account but no order was created, your bank normally <b>reverses it automatically within 5–7 working days</b>.</li>
                    <li>Still not sorted? Email us your registered email, the date, the amount and the transaction reference, and we'll check with PayU for you.</li>
                </ul>

                <h2 id="refunds">7. Refunds</h2>
                <p>When a refund is due to your original payment method, for example for an order cancelled before dispatch or a damaged item, it's processed through PayU. It usually reaches your account within <b>5–7 working days</b>, depending on your bank. Returns for a change of mind are refunded as store credit. See our <a href="~/shipping-policy">Shipping &amp; Returns</a> policy for details.</p>

                <h2 id="safety">8. Stay safe</h2>
                <p class="note-box"><i class="fa fa-shield" aria-hidden="true"></i> <b>India Bobbles will never ask for your OTP, UPI PIN, CVV or card details</b> by phone, email, chat or WhatsApp. If anyone claiming to be us asks for these, don't share them. Report it to us instead.</p>

                <h2 id="contact">9. Contact us</h2>
                <div class="contact-box d-flex justify-content-between align-items-center flex-wrap gap-3">
                    <div>
                        <p class="mb-1"><b>Questions about a payment?</b></p>
                        <p class="mb-0">
                            <i class="fa fa-envelope" aria-hidden="true"></i> <a href="mailto:indiabobbles@rudrasofttech.com?subject=Payment%20query">indiabobbles@rudrasofttech.com</a><br />
                            <i class="fa fa-clock-o" aria-hidden="true"></i> Monday–Saturday, 10:00 AM–6:00 PM IST
                        </p>
                    </div>
                    <a class="btn btn-help" href="https://indiabobbles.tawk.help" target="_blank" rel="noopener"><i class="fa fa-question-circle" aria-hidden="true"></i> Visit Help Centre</a>
                </div>
                <p class="small text-muted mt-3 mb-0">See also our <a href="~/terms-and-conditions">Terms &amp; Conditions</a> and <a href="~/privacy-policy">Privacy Policy</a>.</p>
            </div>
        </div>
    </div>
</div>