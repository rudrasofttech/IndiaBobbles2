@Code
    ViewData("Title") = "Contact Us – India Bobbles"
End Code

@section meta
    <meta name="description" content="Get in touch with India Bobbles. We'd love to hear from you! Reach out to us for bobblehead orders, queries, or feedback." />
    <meta name="Keywords" Content="contact india bobbles, bobblehead order, india bobbles address" />
End Section

<style>
    .email-reveal::before {
        content: attr(data-user);
    }

    .email-reveal::after {
        content: attr(data-domain);
    }
</style>

<div class="container">
    <div class="row" style="background-color:white;box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);">
        <div class="col-sm-12 text-justify mt-2 mb-4">
            <h2 class="text-center mt-3">Contact Us</h2>
            <p class="text-center text-muted">Have a question or want to place a order? We'd love to hear from you!</p>
            <hr />

            <div class="row mt-4">
                <!-- Contact Details -->
                <div class="col-md-6">
                    <h4><i class="fa fa-map-marker fa-fw" style="color:#DC1019;" aria-hidden="true"></i> Our Address</h4>
                    <address class="ml-4">
                        <strong>Raj Kiran Singh</strong><br />
                        India Bobbles<br />
                        H104, Ajnara Daffodil,<br />
                        Sector 137, Noida,<br />
                        Uttar Pradesh – 201305<br />
                        India
                    </address>

                    <h4 class="mt-4"><i class="fa fa-envelope fa-fw" style="color:#DC1019;" aria-hidden="true"></i> Email Us</h4>
                    <p class="ml-4">
                        <span class="email-reveal"
                              data-user="ib&#64;"
                              data-domain="rudrasofttech&#46;com"
                              title="Email us">
                        </span>
                    </p>

                    <h4 class="mt-4"><i class="fa fa-whatsapp fa-fw" style="color:#DC1019;" aria-hidden="true"></i> WhatsApp Us</h4>
                    <p class="ml-4">
                        <a href="@Url.Action("WhatsAppRedirect", "Home")"
                           target="_blank" rel="noopener noreferrer">
                            Click to chat on WhatsApp
                        </a>
                    </p>

                    <h4 class="mt-4"><i class="fa fa-clock-o fa-fw" style="color:#DC1019;" aria-hidden="true"></i> Business Hours</h4>
                    <p class="ml-4">
                        Monday – Saturday: 10:00 AM – 6:00 PM IST<br />
                        Sunday: Closed
                    </p>
                </div>

            </div>
        </div>
    </div>
</div>