namespace Osirion.Blazor.Example.Bootstrap.Components.Pages;

public partial class Showcase
{
    private static readonly ShowcaseSite[] Sites =
    [
        new(
            "Tridesetri",
            "https://tridesetri.com",
            "/assets/images/showcase/tridesetri.webp",
            "The top of the Tridesetri home page in the light theme: a menu with Home, Services, Projects and Company, a Contact Us button and a theme toggle, then the headline \"Tridesetri - Precision Crafted Software\" with two buttons beside a photo of code on a screen.",
            [
                "Tridesetri is a software company in Belgrade. Its home page calls it \"Precision Crafted Software\" and says it builds \"solutions that meet your specific needs\". It lists three services: search engine optimization, web development and custom software development. It also lists three products: Hexavera, Serpence and Kronos.",
            ],
            "the menu with submenus, the light and dark theme toggle and the cookie banner with Decline, Customize and Accept All."),
        new(
            "Hexavera",
            "https://hexavera.com",
            "/assets/images/showcase/hexavera.webp",
            "The top of the Hexavera home page in the light theme: a green announcement bar with a link to a live demo, the logo and a menu button, then the headline \"The workforce intelligence platform\" with the buttons \"Request an executive briefing\" and \"Try the live demo\".",
            [
                "Hexavera describes itself as \"workforce intelligence for manufacturing\": a platform that connects workforce, attendance, production and financial context \"without replacing the systems you already run\". Its pages name modules such as workforce planning, access control, time and attendance, reporting and analytics, and a mobile app, and the site offers a live demo.",
            ],
            "the announcement bar above the menu, which a visitor can close, the collapsible menu and the cookie banner."),
        new(
            "demonjic.com",
            "https://demonjic.com",
            "/assets/images/showcase/demonjic.webp",
            "The top of the demonjic.com home page in the light theme: the name Dejan Demonjić and a menu with Decision log, Work, About and Contact, then the headline \"Secure, multi-tenant .NET platforms, and the decisions behind them.\" with the buttons \"Read the log\" and \"See the work\".",
            [
                "demonjic.com is the personal site of Dejan Demonjić, who presents himself as a principal software engineer in Belgrade. Its main section is a \"Decision log\", a blog of technical write-ups on secure, multi-tenant .NET platforms, grouped in series. It also has a Work page that lists Hexavera and Osirion.Blazor among his projects, and an RSS feed.",
            ],
            "the blog, the menu, the light and dark theme toggle and an RSS feed. The page's generator tag reads \"Osirion Blazor CMS\"."),
    ];

    private sealed record ShowcaseSite(string Name, string Url, string Image, string ImageAlt, string[] Description, string Features);
}
