@Code
    ViewData("Title") = "Custom Bobbleheads from Your Photo – Hand-painted | India Bobbles"
    Dim waBase As String = Url.Action("WhatsAppRedirect", "Home")
End Code

@section meta
    <meta name="description" content="Order a custom bobblehead made from your photo. Hand-painted by master artists in India. Perfect for weddings, anniversaries, birthdays and corporate gifts. Free shipping across India." />
End Section

@section head
    <style>
        
    </style>
End Section

@section scripts
    <script>
        function ibCustomEnquiry() {
            var name = $('#cName').val().trim();
            var occasion = $('#cOccasion').val();
            var people = $('#cPeople').val();
            var date = $('#cDate').val();
            var idea = $('#cIdea').val().trim();
            var msg = $('#cMsg');

            if (name === '') {
                msg.text('Please enter your name.').css('color', '#ffb3b3');
                return;
            }

            var text = 'Hi India Bobbles! I would like a custom bobblehead.\n' +
                'Name: ' + name + '\n' +
                'Occasion: ' + occasion + '\n' +
                'Number of people: ' + people + '\n' +
                (date ? 'Needed by: ' + date + '\n' : '') +
                (idea ? 'My idea: ' + idea + '\n' : '') +
                'I will share my photos here.';

            msg.text('Opening WhatsApp… please attach your photos there.').css('color', '#ffc107');
            window.open('@waBase' + '?message=' + encodeURIComponent(text), '_blank');
        }
    </script>
End Section

<div class="ib-cust">

    <!-- ============ Hero ============ -->
    <section class="hero">
        <div class="container py-5">
            <div class="row align-items-center g-4">
                <div class="col-lg-6">
                    <div class="kick">Custom bobbleheads · Hand-painted in India</div>
                    <h1>Your face. Our artists. <em>One-of-a-kind.</em></h1>
                    <p class="mb-4">Send us a photo and our master artists will sculpt and hand-paint a bobblehead of you, your partner, your parents or your boss. A gift nobody else can give.</p>
                    <div class="d-flex flex-wrap gap-2">
                        <a class="btn btn-iby" href="#enquire">Get a Free Quote <i class="fa fa-arrow-right" aria-hidden="true"></i></a>
                        <a class="btn btn-ibw" href="#how">How it works</a>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="transform">
                        <div class="photo"><i class="fa fa-camera" aria-hidden="true"></i>Your photo</div>
                        <div class="arrow"><i class="fa fa-long-arrow-right" aria-hidden="true"></i></div>
                        @* Replace with a photo of a real custom bobblehead you have made *@
                        <div class="bobble"><img src="/drive/theme/khichdi/img/daku-product-thumbnail.jpg" alt="Hand-painted bobblehead" /></div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="strip">
        <div class="container d-flex flex-wrap justify-content-around gap-3 py-3">
            <span><i class="fa fa-paint-brush" aria-hidden="true"></i>Sculpted &amp; hand-painted</span>
            <span><i class="fa fa-comments" aria-hidden="true"></i>Free quote, no obligation</span>
            <span><i class="fa fa-check-circle" aria-hidden="true"></i>You approve the design</span>
            <span><i class="fa fa-truck" aria-hidden="true"></i>Free shipping across India</span>
        </div>
    </div>

    <!-- ============ How it works ============ -->
    <section class="sec" id="how">
        <div class="container">
            <div class="text-center mb-5">
                <h2>How it works</h2>
                <p class="lead-sm mb-0">Four simple steps from photo to bobblehead.</p>
            </div>
            <div class="row g-4">
                <div class="col-sm-6 col-lg-3"><div class="step"><span class="num">1</span><i class="fa fa-camera" aria-hidden="true"></i><h5>Send your photos</h5><p>Share 2–3 clear photos on WhatsApp or email, with your idea and deadline.</p></div></div>
                <div class="col-sm-6 col-lg-3"><div class="step"><span class="num">2</span><i class="fa fa-inr" aria-hidden="true"></i><h5>Get a free quote</h5><p>We confirm the price and timeline. No payment until you're happy with the quote.</p></div></div>
                <div class="col-sm-6 col-lg-3"><div class="step"><span class="num">3</span><i class="fa fa-paint-brush" aria-hidden="true"></i><h5>We sculpt &amp; paint</h5><p>Our artists sculpt the likeness and hand-paint every detail. You approve the design.</p></div></div>
                <div class="col-sm-6 col-lg-3"><div class="step"><span class="num">4</span><i class="fa fa-gift" aria-hidden="true"></i><h5>Delivered free</h5><p>Carefully packed and shipped free anywhere in India. Ready to gift.</p></div></div>
            </div>
        </div>
    </section>

    <!-- ============ Occasions ============ -->
    <section class="sec pt-0">
        <div class="container">
            <h2>Perfect for every occasion</h2>
            <p class="lead-sm mb-4">A bobblehead is a gift people keep on display for years.</p>
            <div class="row g-3">
                <div class="col-6 col-md-4 col-lg-2"><div class="occ"><span class="e">💍</span><b>Weddings</b><span>Couple bobbleheads</span></div></div>
                <div class="col-6 col-md-4 col-lg-2"><div class="occ"><span class="e">❤️</span><b>Anniversaries</b><span>Celebrate together</span></div></div>
                <div class="col-6 col-md-4 col-lg-2"><div class="occ"><span class="e">🎂</span><b>Birthdays</b><span>A gift with a face</span></div></div>
                <div class="col-6 col-md-4 col-lg-2"><div class="occ"><span class="e">👨‍👩‍👧</span><b>Parents</b><span>For Mom &amp; Dad</span></div></div>
                <div class="col-6 col-md-4 col-lg-2"><div class="occ"><span class="e">🏢</span><b>Corporate</b><span>Farewells &amp; awards</span></div></div>
                <div class="col-6 col-md-4 col-lg-2"><div class="occ"><span class="e">🪔</span><b>Festivals</b><span>Diwali &amp; more</span></div></div>
            </div>
        </div>
    </section>

    <!-- ============ What you can customise + photo tips ============ -->
    <section class="sec pt-0">
        <div class="container">
            <div class="row g-4 align-items-start">
                <div class="col-lg-6">
                    <h2>What you can customise</h2>
                    <ul class="opt ps-0 mt-3">
                        <li><i class="fa fa-user" aria-hidden="true"></i> The face, hairstyle, beard and glasses</li>
                        <li><i class="fa fa-black-tie" aria-hidden="true"></i> Outfit: suit, sherwani, saree, uniform, jersey</li>
                        <li><i class="fa fa-music" aria-hidden="true"></i> Props: cricket bat, guitar, laptop, stethoscope</li>
                        <li><i class="fa fa-users" aria-hidden="true"></i> One person, a couple or a whole group</li>
                        <li><i class="fa fa-font" aria-hidden="true"></i> A name or message on the base</li>
                    </ul>
                </div>
                <div class="col-lg-6">
                    <div class="tips">
                        <h5 class="fw-bold mb-3"><i class="fa fa-lightbulb-o" aria-hidden="true"></i> Photo tips for the best likeness</h5>
                        <ul class="mb-0 ps-3">
                            <li>Send <b>2–3 photos</b>: one from the front, one from the side.</li>
                            <li>Use <b>good lighting</b> with the face clearly visible.</li>
                            <li>No sunglasses, hats over the face or heavy filters.</li>
                            <li>Higher resolution works best. Send the original, not a screenshot.</li>
                            <li>Send a separate photo of any outfit or prop you want.</li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- ============ Enquiry ============ -->
    <section class="sec pt-0" id="enquire">
        <div class="container">
            <div class="enq">
                <div class="row g-4">
                    <div class="col-lg-5">
                        <h2>Get your free quote</h2>
                        <p class="sub">Fill in a few details, tap the button, and WhatsApp opens with your message ready. Just attach your photos and send.</p>
                        <p class="note mb-1"><i class="fa fa-envelope" aria-hidden="true"></i> Prefer email? Send your photos and idea to <a href="mailto:indiabobbles@rudrasofttech.com?subject=Custom%20bobblehead%20enquiry">indiabobbles@rudrasofttech.com</a></p>
                        <p class="note"><i class="fa fa-clock-o" aria-hidden="true"></i> We reply Monday–Saturday, 10 AM–6 PM IST.</p>
                    </div>
                    <div class="col-lg-7">
                        <div class="row g-3">
                            <div class="col-md-6">
                                <label for="cName">Your name</label>
                                <input type="text" id="cName" class="form-control" maxlength="100" placeholder="e.g. Priya Sharma" />
                            </div>
                            <div class="col-md-6">
                                <label for="cOccasion">Occasion</label>
                                <select id="cOccasion" class="form-select">
                                    <option>Wedding</option>
                                    <option>Anniversary</option>
                                    <option>Birthday</option>
                                    <option>Gift for parents</option>
                                    <option>Corporate gift / farewell</option>
                                    <option>Festival gift</option>
                                    <option>Just for me</option>
                                </select>
                            </div>
                            <div class="col-md-6">
                                <label for="cPeople">Number of people</label>
                                <select id="cPeople" class="form-select">
                                    <option>1 person</option>
                                    <option>2 people (couple)</option>
                                    <option>3 or more</option>
                                </select>
                            </div>
                            <div class="col-md-6">
                                <label for="cDate">Needed by (optional)</label>
                                <input type="date" id="cDate" class="form-control" />
                            </div>
                            <div class="col-12">
                                <label for="cIdea">Your idea (optional)</label>
                                <textarea id="cIdea" class="form-control" rows="3" maxlength="500" placeholder="e.g. Husband in a cricket jersey holding a bat, 'Happy 10th Anniversary' on the base"></textarea>
                            </div>
                            <div class="col-12">
                                <button type="button" class="btn btn-wa w-100" onclick="ibCustomEnquiry()"><i class="fa fa-whatsapp" aria-hidden="true"></i> Send enquiry on WhatsApp</button>
                                <div id="cMsg" class="small mt-2"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- ============ FAQ ============ -->
    <section class="sec pt-0">
        <div class="container" style="max-width:860px;">
            <h2 class="text-center mb-4">Questions? We've got answers</h2>
            <div class="accordion" id="custFaq">
                <div class="accordion-item">
                    <h3 class="accordion-header"><button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#f1">How much does a custom bobblehead cost?</button></h3>
                    <div id="f1" class="accordion-collapse collapse show" data-bs-parent="#custFaq"><div class="accordion-body">The price depends on the number of people, outfit and props. Send us your photos and idea for a free quote. Shipping is always free within India.</div></div>
                </div>
                <div class="accordion-item">
                    <h3 class="accordion-header"><button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#f2">How long does it take?</button></h3>
                    <div id="f2" class="accordion-collapse collapse" data-bs-parent="#custFaq"><div class="accordion-body">We confirm the timeline with your quote. For weddings, festivals and birthdays, please contact us early and tell us your deadline.</div></div>
                </div>
                <div class="accordion-item">
                    <h3 class="accordion-header"><button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#f3">Will I see the design before it's finished?</button></h3>
                    <div id="f3" class="accordion-collapse collapse" data-bs-parent="#custFaq"><div class="accordion-body">Yes. We share the design with you for approval before we finish and ship your bobblehead.</div></div>
                </div>
                <div class="accordion-item">
                    <h3 class="accordion-header"><button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#f4">Can I return a custom bobblehead?</button></h3>
                    <div id="f4" class="accordion-collapse collapse" data-bs-parent="#custFaq"><div class="accordion-body">Custom bobbleheads are made just for you, so they can't be returned. If it arrives damaged, email photos to <a href="mailto:indiabobbles@rudrasofttech.com">indiabobbles@rudrasofttech.com</a> within 4 hours of delivery and we'll make it right.</div></div>
                </div>
                <div class="accordion-item">
                    <h3 class="accordion-header"><button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#f5">Do you take bulk or corporate orders?</button></h3>
                    <div id="f5" class="accordion-collapse collapse" data-bs-parent="#custFaq"><div class="accordion-body">Yes. Tell us the quantity, design and delivery date in your enquiry and we'll send a quote.</div></div>
                </div>
            </div>
            <p class="text-center mt-4 mb-0">Not ready for custom? <a href="~/tag/collectibles" style="color:#330B3F;font-weight:700;">Browse our ready-made bobbleheads <i class="fa fa-arrow-right" aria-hidden="true"></i></a></p>
        </div>
    </section>
</div>