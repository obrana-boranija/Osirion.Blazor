# OsirionFooter

Purpose
Composable footer with sections, social links and bottom links. Docking support for short pages.

Key parameters
- CompanyName, CompanyUrl, Description
- Logo (RenderFragment), LeftContent, RightContent
- Links: IReadOnlyList<FooterSection>
- SocialLinks: IReadOnlyList<FooterSocialLink>, SocialTitle, ShowSocialLinks
- BottomLinks: IReadOnlyList<FooterLink>
- Variant: default, minimal, centered
- GridLayout: auto, 4-column, 3-column, 2-column
- Docked (bool), DockingMode: fixed or sticky

Models
- FooterSection { Title, Links[] }
- FooterLink { Text, Href, OpenInNewTab }
- FooterSocialLink : FooterLink { Icon, AriaLabel }
