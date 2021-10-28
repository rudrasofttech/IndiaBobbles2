Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports System.Web.Routing

Public Module RouteConfig
    Public Sub RegisterRoutes(ByVal routes As RouteCollection)
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")
        routes.MapRoute(
            name:="Games",
            url:="games",
            defaults:=New With {.controller = "Home", .action = "Games", .id = UrlParameter.Optional}
        )
        routes.MapRoute(
            name:="About",
            url:="about",
            defaults:=New With {.controller = "Home", .action = "About", .id = UrlParameter.Optional}
        )
        routes.MapRoute(
            name:="Terms",
            url:="terms-and-conditions",
            defaults:=New With {.controller = "Home", .action = "Terms", .id = UrlParameter.Optional}
        )
        routes.MapRoute(
            name:="Privacy",
            url:="privacy-policy",
            defaults:=New With {.controller = "Home", .action = "Privacy", .id = UrlParameter.Optional}
        )
        routes.MapRoute(
            name:="Shipping",
            url:="shipping-policy",
            defaults:=New With {.controller = "Home", .action = "Shipping", .id = UrlParameter.Optional}
        )
        routes.MapRoute(
            name:="PaymentOptions",
            url:="payment-options",
            defaults:=New With {.controller = "Home", .action = "Payment", .id = UrlParameter.Optional}
        )
        routes.MapRoute(
            name:="Collectibles",
            url:="collectibles",
            defaults:=New With {.controller = "Home", .action = "Collectibles", .id = UrlParameter.Optional}
        )
        routes.MapRoute(
            name:="dakusambharsinghbobblehead",
            url:="daku-sambhar-singh-bobble-head",
            defaults:=New With {.controller = "Product", .action = "Detail", .id = "daku-sambhar-singh-bobble-head"}
        )
        routes.MapRoute(
            name:="chhote-bachchan-bobblehead",
            url:="chhote-bachchan-bobblehead",
            defaults:=New With {.controller = "Product", .action = "Detail", .id = "chhote-bachchan-bobblehead"}
        )
        routes.MapRoute(
            name:="nazarbattu-bobblehead",
            url:="nazarbattu-bobblehead",
            defaults:=New With {.controller = "Product", .action = "Detail", .id = "nazarbattu-bobblehead"}
        )
        routes.MapRoute(
            name:="bhagat-singh-figurine",
            url:="bhagat-singh-figurine",
            defaults:=New With {.controller = "Product", .action = "Detail", .id = "bhagat-singh-figurine"}
        )
        routes.MapRoute(
            name:="mahatma-gandhi-bobblehead",
            url:="mahatma-gandhi-bobblehead",
            defaults:=New With {.controller = "Product", .action = "Detail", .id = "mahatma-gandhi-bobblehead"}
        )
        routes.MapRoute(
            name:="dr-abdul-kalam-bobblehead",
            url:="dr-abdul-kalam-bobblehead",
            defaults:=New With {.controller = "Product", .action = "Detail", .id = "dr-abdul-kalam-bobblehead"}
        )
        routes.MapRoute(
            name:="android-figurine",
            url:="android-figurine",
            defaults:=New With {.controller = "Product", .action = "Detail", .id = "android-figurine"}
        )
        routes.MapRoute(
            name:="master-blaster-bobblehead",
            url:="master-blaster-bobblehead",
            defaults:=New With {.controller = "Product", .action = "Detail", .id = "master-blaster-bobblehead"}
        )
        routes.MapRoute(
            name:="CustomBobblehead",
            url:="order-custom-bobbleheads",
            defaults:=New With {.controller = "Home", .action = "CustomBobblehead", .id = UrlParameter.Optional}
        )
        routes.MapRoute(
            name:="Blog",
            url:="blog",
            defaults:=New With {.controller = "Articles", .action = "Index", .id = UrlParameter.Optional}
        )
        routes.MapRoute(
            name:="BlogPost",
            url:="blog/{url}",
            defaults:=New With {.controller = "Articles", .action = "Detail", .url = UrlParameter.Optional}
        )
        routes.MapRoute(
            name:="Default",
            url:="{controller}/{action}/{id}",
            defaults:=New With {.controller = "Home", .action = "Index", .id = UrlParameter.Optional}
        )
    End Sub
End Module