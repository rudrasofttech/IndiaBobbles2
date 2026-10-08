@Code
    ViewData("Title") = "Privacy Policy – India Bobbles"
End Code

@section meta
    <meta name="description" content="How India Bobbles collects, uses, shares and protects your personal data, and your rights under India's Digital Personal Data Protection Act, 2023." />
End Section

@section head
    <style>
        .ib-legal {
            --p: #330B3F;
            --y: #ffc107;
            --y2: #fff4cc;
            --mut: #6b5f72;
            --line: #ece6ef;
            --bg: #faf8fb;
            padding-top: 28px;
            padding-bottom: 56px;
        }

            .ib-legal .hero {
                background: linear-gradient(90deg,#330B3F,#4d1460);
                color: #fff;
                border-radius: 16px;
                padding: 28px 32px;
                margin-bottom: 28px;
            }

                .ib-legal .hero h1 {
                    font-weight: 800;
                    margin: 0 0 6px;
                    font-size: 2.1rem;
                }

                .ib-legal .hero p {
                    color: #e2d3ea;
                    margin: 0;
                }

                .ib-legal .hero .upd {
                    display: inline-block;
                    background: var(--y);
                    color: var(--p);
                    font-weight: 700;
                    font-size: .8rem;
                    padding: 4px 10px;
                    border-radius: 20px;
                    margin-top: 12px;
                }

            .ib-legal .toc {
                position: sticky;
                top: 110px;
                border: 1px solid var(--line);
                border-radius: 14px;
                padding: 18px;
                background: #fff;
                max-height: calc(100vh - 140px);
                overflow: auto;
            }

                .ib-legal .toc h6 {
                    font-weight: 800;
                    color: var(--p);
                    text-transform: uppercase;
                    font-size: .75rem;
                    letter-spacing: .1em;
                }

                .ib-legal .toc ol {
                    padding-left: 1.1rem;
                    margin: 0;
                    font-size: .88rem;
                }

                .ib-legal .toc li {
                    margin-bottom: 6px;
                }

                .ib-legal .toc a {
                    color: #1f1724;
                    text-decoration: none;
                }

                    .ib-legal .toc a:hover {
                        color: var(--p);
                        text-decoration: underline;
                    }

            .ib-legal .doc {
                font-size: 1rem;
                line-height: 1.75;
                color: #2b2230;
            }

                .ib-legal .doc h2 {
                    font-size: 1.3rem;
                    font-weight: 800;
                    color: var(--p);
                    margin: 34px 0 10px;
                    scroll-margin-top: 120px;
                }

                .ib-legal .doc h3 {
                    font-size: 1.02rem;
                    font-weight: 700;
                    color: #1f1724;
                    margin: 18px 0 6px;
                }

                .ib-legal .doc a {
                    color: var(--p);
                    font-weight: 600;
                }

            .ib-legal .summary {
                background: var(--y2);
                border-radius: 14px;
                padding: 20px 22px;
                color: var(--p);
            }

                .ib-legal .summary h2 {
                    margin-top: 0;
                }

                .ib-legal .summary ul {
                    margin-bottom: 0;
                }

            .ib-legal table {
                width: 100%;
                border-collapse: collapse;
                font-size: .92rem;
                margin: 10px 0 6px;
            }

            .ib-legal th {
                background: var(--bg);
                color: var(--p);
                text-align: left;
                padding: 10px;
                border: 1px solid var(--line);
            }

            .ib-legal td {
                padding: 10px;
                border: 1px solid var(--line);
                vertical-align: top;
            }

            .ib-legal .contact-box {
                border: 2px solid var(--p);
                border-radius: 14px;
                padding: 20px 22px;
            }

        @@media (max-width: 991.98px) {
            .ib-legal .toc {
                position: static;
                max-height: none;
                margin-bottom: 10px;
            }
        }

        @@media (max-width: 575.98px) {
            .ib-legal .table-wrap {
                overflow-x: auto;
            }

            .ib-legal .hero {
                padding: 22px;
            }

                .ib-legal .hero h1 {
                    font-size: 1.6rem;
                }
        }
    </style>
End Section

<div class="ib-legal container fullbody">
    <div class="hero">
        <h1>Privacy Policy</h1>
        <p>How we collect, use and protect your personal data, and the choices you have.</p>
        <span class="upd">Last updated: 8 October 2026</span>
    </div>

    <div class="row g-4">
        <div class="col-lg-3">
            <nav class="toc" aria-label="Contents">
                <h6>Contents</h6>
                <ol>
                    <li><a href="#who">Who we are</a></li>
                    <li><a href="#scope">Scope of this policy</a></li>
                    <li><a href="#collect">Data we collect</a></li>
                    <li><a href="#use">How we use your data</a></li>
                    <li><a href="#consent">Consent &amp; legal basis</a></li>
                    <li><a href="#marketing">Emails &amp; marketing</a></li>
                    <li><a href="#share">Who we share data with</a></li>
                    <li><a href="#cookies">Cookies &amp; tracking</a></li>
                    <li><a href="#payments">Payments</a></li>
                    <li><a href="#retention">How long we keep data</a></li>
                    <li><a href="#security">Security</a></li>
                    <li><a href="#rights">Your rights</a></li>
                    <li><a href="#children">Children</a></li>
                    <li><a href="#transfer">Data stored outside India</a></li>
                    <li><a href="#links">Links to other sites</a></li>
                    <li><a href="#breach">Data breaches</a></li>
                    <li><a href="#changes">Changes to this policy</a></li>
                    <li><a href="#contact">Grievance Officer &amp; contact</a></li>
                </ol>
            </nav>
        </div>

        <div class="col-lg-9">
            <div class="doc">

                <div class="summary">
                    <h2>The short version</h2>
                    <ul>
                        <li>We collect only what we need to run your account, deliver your orders and (if you agree) send you offers.</li>
                        <li>We <b>never sell</b> your personal data.</li>
                        <li>We <b>don't store</b> your card or bank details. Payments are handled by our payment gateway.</li>
                        <li>Marketing emails are <b>opt-in</b>, and you can unsubscribe anytime.</li>
                        <li>You can ask us to see, correct or delete your data at any time.</li>
                    </ul>
                </div>

                <h2 id="who">1. Who we are</h2>
                <p>India Bobbles ("<b>India Bobbles</b>", "<b>we</b>", "<b>us</b>") sells hand-painted collectible bobbleheads and figurines through <a href="https://www.indiabobbles.com">www.indiabobbles.com</a> (the "<b>Website</b>"). The Website is operated by <b>[Legal entity name]</b>, H104, Ajnara Daffodil, Sector 137, Noida, Uttar Pradesh – 201305, India.</p>
                <p>For the purposes of India's Digital Personal Data Protection Act, 2023 ("<b>DPDP Act</b>") and the rules made under it, we are the <b>Data Fiduciary</b> for the personal data described in this policy. You are the <b>Data Principal</b>.</p>

                <h2 id="scope">2. Scope of this policy</h2>
                <p>This policy applies to personal data we collect when you visit the Website, create an account, place an order, chat with us, comment on our blog, subscribe to emails, contact us by email or WhatsApp, or read our Help Centre at indiabobbles.tawk.help. It does not apply to other websites you reach through links on our Website.</p>
                <p>By using the Website, you acknowledge this policy. Where we rely on your consent, we ask for it separately and clearly.</p>

                <h2 id="collect">3. Data we collect</h2>
                <h3>a) Data you give us</h3>
                <ul>
                    <li><b>Account details:</b> name, email address, mobile number and, if you choose to add them, last name, date of birth, gender and country.</li>
                    <li><b>Login data:</b> one-time passcodes (OTPs) sent to your email. We don't use or store passwords.</li>
                    <li><b>Order details:</b> delivery name, address, PIN code, phone number, the products you buy and your order history.</li>
                    <li><b>Custom bobblehead requests:</b> the photos, descriptions, occasion and deadline you send us. Photos may show you or other people. Please only send photos of others with their permission.</li>
                    <li><b>Communications:</b> messages you send through our chat, email, WhatsApp, contact forms or blog comments, including any files you attach.</li>
                    <li><b>Preferences:</b> whether you want marketing emails, and "notify me" requests for out-of-stock products.</li>
                </ul>
                <h3>b) Data collected automatically</h3>
                <ul>
                    <li><b>Device and usage data:</b> IP address, browser type, device type, pages viewed, time spent, referring website and approximate location (city or region) derived from your IP address.</li>
                    <li><b>Cookies and similar technologies:</b> see <a href="#cookies">section 8</a>.</li>
                </ul>
                <h3>c) Data from others</h3>
                <ul>
                    <li><b>Payment gateway:</b> confirmation of whether your payment succeeded, a transaction reference and the payment method type. We do not receive your full card number or bank login.</li>
                    <li><b>Courier partners:</b> delivery status and proof of delivery.</li>
                </ul>

                <h2 id="use">4. How we use your data</h2>
                <div class="table-wrap">
                    <table>
                        <tr><th style="width:38%">Purpose</th><th>Data used</th></tr>
                        <tr><td>Create and manage your account; log you in with an OTP</td><td>Name, email, mobile, login records</td></tr>
                        <tr><td>Process, pack, ship and deliver orders; handle returns, refunds and replacements</td><td>Contact and address details, order details, payment confirmation</td></tr>
                        <tr><td>Make custom bobbleheads and send quotes</td><td>Photos and details you provide</td></tr>
                        <tr><td>Customer support (chat, email, WhatsApp)</td><td>Contact details, messages, order details</td></tr>
                        <tr><td>Send order, delivery and account messages (these are not marketing)</td><td>Email, mobile</td></tr>
                        <tr><td>Send offers, new launches and newsletters <b>(only with your consent)</b></td><td>Name, email, date of birth (for birthday offers, if given), purchase history</td></tr>
                        <tr><td>Tell you when an out-of-stock product is back</td><td>Email</td></tr>
                        <tr><td>Improve the Website and understand what visitors like</td><td>Usage data, cookies (analytics)</td></tr>
                        <tr><td>Prevent fraud, spam and misuse (for example captcha checks)</td><td>IP address, device data, account activity</td></tr>
                        <tr><td>Meet legal, tax and accounting obligations, and respond to lawful requests</td><td>Order and invoice records</td></tr>
                    </table>
                </div>
                <p>We do not use your data for automated decisions that have legal or similarly significant effects on you.</p>

                <h2 id="consent">5. Consent &amp; legal basis</h2>
                <p>Under the DPDP Act, we process your personal data either:</p>
                <ul>
                    <li><b>with your consent</b>, for example when you create an account, subscribe to marketing emails, send us photos for a custom order or post a blog comment; or</li>
                    <li><b>for certain legitimate uses</b> permitted by law, for example where you voluntarily give us data for a specific purpose (such as delivering an order you placed), or to comply with a law, court order or government request.</li>
                </ul>
                <p><b>Withdrawing consent:</b> you can withdraw your consent at any time, as easily as you gave it, by updating your <a href="~/account/manageprofile">profile</a>, using the unsubscribe link in any email, or writing to our Grievance Officer (<a href="#contact">section 18</a>). Withdrawal doesn't affect processing already done, and we may still keep data where the law requires it. If you withdraw consent needed to run your account, we may not be able to continue providing it.</p>

                <h2 id="marketing">6. Emails &amp; marketing</h2>
                <ul>
                    <li>We send marketing emails (offers, launches, festival deals, newsletters) <b>only if you opt in</b>, for example by ticking the box when you register, switching it on in your profile, or subscribing on our Website.</li>
                    <li>Every marketing email includes an <b>unsubscribe</b> link. You can also turn emails off in your profile.</li>
                    <li>Service messages about your account, orders and deliveries will still be sent, because we need them to serve you.</li>
                    <li>We may use an email service provider to send emails. They act only on our instructions.</li>
                </ul>

                <h2 id="share">7. Who we share data with</h2>
                <p><b>We never sell or rent your personal data.</b> We share it only with service providers (<b>Data Processors</b>) who help us run our business, under contracts that require them to protect it and use it only for our purposes:</p>
                <div class="table-wrap">
                    <table>
                        <tr><th style="width:30%">Who</th><th>Why</th></tr>
                        <tr><td>Payment gateway (PayU)</td><td>To process your payment securely</td></tr>
                        <tr><td>Courier partners (such as DTDC)</td><td>To deliver your order: name, address, phone number</td></tr>
                        <tr><td>Live chat &amp; help centre (tawk.to)</td><td>To provide chat support and our Help Centre</td></tr>
                        <tr><td>WhatsApp (Meta)</td><td>When you choose to message us on WhatsApp</td></tr>
                        <tr><td>Blog comments (Disqus)</td><td>When you comment on our blog. Disqus has its own privacy policy.</td></tr>
                        <tr><td>Analytics (Google Analytics and web statistics tools)</td><td>To understand how the Website is used</td></tr>
                        <tr><td>Social media (Facebook / Instagram)</td><td>Social plugins and links on our pages</td></tr>
                        <tr><td>Hosting, email and IT providers</td><td>To host the Website, store data and send emails, including OTPs</td></tr>
                        <tr><td>Professional advisers</td><td>Accountants, auditors and lawyers, where needed</td></tr>
                    </table>
                </div>
                <p>We may also disclose data where required by law, a court order or a government authority; to protect our rights, customers or the public from fraud or harm; or to a successor if our business is sold or merged, in which case this policy will continue to apply.</p>

                <h2 id="cookies">8. Cookies &amp; tracking</h2>
                <p>Cookies are small files stored on your device. We use:</p>
                <ul>
                    <li><b>Essential cookies:</b> to keep you logged in, remember your cart and keep the Website secure. The Website won't work properly without them.</li>
                    <li><b>Analytics cookies:</b> to count visits and see which pages are popular (for example Google Analytics).</li>
                    <li><b>Third-party cookies:</b> set by services embedded on our pages, such as live chat, blog comments and social media plugins.</li>
                </ul>
                <p>You can block or delete cookies in your browser settings. Blocking essential cookies may stop parts of the Website, such as login and cart, from working.</p>

                <h2 id="payments">9. Payments</h2>
                <p>Online payments are processed by our payment gateway partner, PayU. Your card, UPI and bank details are entered on, or securely passed to, the gateway. <b>We do not see or store your full card number, CVV, UPI PIN or net banking password.</b> The gateway handles this data under its own security standards and privacy policy.</p>

                <h2 id="retention">10. How long we keep data</h2>
                <ul>
                    <li><b>Account data:</b> while your account is active. If you ask us to delete your account, we erase your personal data unless we must keep it by law.</li>
                    <li><b>Inactive accounts:</b> we may delete accounts that haven't been used for 3 years. We'll email you at least 48 hours before deleting, so you can log in to keep it.</li>
                    <li><b>Order and invoice records:</b> as long as tax, accounting and consumer laws require, generally up to 8 years.</li>
                    <li><b>Custom order photos:</b> until your order is complete and any replacement period is over, then deleted, unless you allow us to keep them.</li>
                    <li><b>Marketing preferences:</b> until you unsubscribe. We keep a record of the unsubscribe so we don't email you again.</li>
                    <li><b>Chat and support messages:</b> up to 2 years, to help resolve repeat issues.</li>
                    <li><b>Security and access logs:</b> at least one year, as required by law, to investigate misuse.</li>
                </ul>

                <h2 id="security">11. Security</h2>
                <p>We use reasonable security safeguards to protect your data, including HTTPS encryption, OTP-based login with no stored passwords, captcha and spam checks, access controls that limit who can see data, and regular review of our systems and service providers. No system is completely secure. If you think your account has been misused, contact us immediately.</p>

                <h2 id="rights">12. Your rights</h2>
                <p>Under the DPDP Act, you have the right to:</p>
                <ul>
                    <li><b>Access:</b> get a summary of the personal data we hold about you, how we use it, and who we've shared it with.</li>
                    <li><b>Correct and update:</b> fix inaccurate or incomplete data. You can update most details yourself in <a href="~/account/manageprofile">My Profile</a>.</li>
                    <li><b>Erase:</b> ask us to delete your data when it's no longer needed or you withdraw consent, unless we must keep it by law.</li>
                    <li><b>Withdraw consent:</b> at any time (see <a href="#consent">section 5</a>).</li>
                    <li><b>Grievance redressal:</b> complain to our Grievance Officer, who will respond within the time required by law.</li>
                    <li><b>Nominate:</b> name another person to exercise your rights if you die or become incapacitated.</li>
                </ul>
                <p>To use any of these rights, email our Grievance Officer (<a href="#contact">section 18</a>). We may need to verify your identity first, usually with an OTP to your registered email. If you're not satisfied with our response, you can complain to the <b>Data Protection Board of India</b>.</p>

                <h2 id="children">13. Children</h2>
                <p>Our Website is meant for adults. Some of our products are suitable for ages 14+, but accounts and orders should be created by a parent or guardian. We do not knowingly collect personal data from anyone under 18 without verifiable consent from a parent or guardian, and we do not track, profile or send targeted ads to children. If you believe a child has given us personal data, contact us and we'll delete it.</p>

                <h2 id="transfer">14. Data stored outside India</h2>
                <p>Some of our service providers, such as chat, analytics, comments and email tools, may store or process data on servers outside India. When this happens, we only transfer data as permitted by Indian law, and we choose providers that protect it with appropriate safeguards.</p>

                <h2 id="links">15. Links to other sites</h2>
                <p>Our Website may contain links to other websites. We do not control, and are not responsible for, the privacy practices of other sites. We encourage you to read the privacy policy of every website that collects personal data. This policy applies only to data collected by India Bobbles.</p>

                <h2 id="breach">16. Data breaches</h2>
                <p>If a personal data breach affects you, we'll inform you without delay, explaining what happened, the likely impact, what we're doing about it and steps you can take to protect yourself. We'll also report it to the Data Protection Board of India and other authorities as required by law.</p>

                <h2 id="changes">17. Changes to this policy</h2>
                <p>We may update this policy from time to time. We'll change the "Last updated" date at the top and, for significant changes, notify you by email or with a notice on the Website. Please check this page regularly.</p>

                <h2 id="contact">18. Grievance Officer &amp; contact</h2>
                <div class="contact-box">
                    <p class="mb-2">For any question, request or complaint about your personal data, contact our Grievance Officer:</p>
                    <p class="mb-2">
                        <b>Name:</b> Raj Kiran Singh<br />
                        <b>Email:</b> <a href="mailto:indiabobbles@rudrasofttech.com">indiabobbles@rudrasofttech.com</a><br />
                        <b>Address:</b> Rudra Softtech LLP, H104, Ajnara Daffodil, Sector 137, Noida, Uttar Pradesh – 201305, India<br />
                        <b>Hours:</b> Monday–Saturday, 10:00 AM–6:00 PM IST
                    </p>
                    <p class="mb-0">We acknowledge complaints within 48 hours and aim to resolve them within one month, and in any case within the time required by law.</p>
                </div>
            </div>
        </div>
    </div>
</div>