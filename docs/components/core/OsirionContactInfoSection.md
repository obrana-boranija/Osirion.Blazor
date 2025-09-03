Purpose
-------
Render a structured contact information block (address, phone, email, website) with accessible labels and icons.

Parameters
----------
- `ContactInfo` (ContactInfoModel) — Model containing Address, Phone, Email, Website and optional Message.
- `Title` (string) — Optional title shown above the contact information.
- `AddressLabel`, `PhoneLabel`, `EmailLabel`, `WebsiteLabel` — Optional labels for each field.

Example
-------
```razor
<OsirionContactInfoSection ContactInfo="@contactModel" Title="Contact us" />
```

Notes
-----
- Links are rendered with proper `mailto:` and `tel:` links and open website links in a new tab.
