# Tailwind styling

Edit the source files here, not `wwwroot/css/site.css`. All three Razor layouts
load that generated stylesheet. Tailwind CSS v4 compiles the shared classes with
`@apply`; Razor views may also use Tailwind utilities directly. Semantic class
names remain reusable and preserve existing JavaScript hooks.

```text
Styles/
  app.css                    Entry point, import order, Razor/JS source discovery
  base/
    theme.css                University colors and fonts
    global.css               Document, typography, focus, and accessibility rules
  components/
    branding.css             Crest and wordmark
    typography.css           Shared public section titles, subheadings, and body scale
    forms.css                Fields, validation, buttons, uploads, editor rows
    navigation.css           Public navbar and account dropdown
  layouts/
    account.css              Login and registration layouts
    admin.css                Admin sidebar, workspace, dashboard
  sections/
    university.css           Hero, contact, founder, content containers
    academics.css            Program cards and manual browse controls
    about.css                Sticky 50/50 text/image chapters and mobile rail
    gallery.css              Stationary responsive photo grid and image dialog
    collegiate.css           Curved rope, five house pennants, responsive banner row
    contact.css              Simple address/hours, contact details, social links
```

Run from the repository root:

```powershell
npm ci
npm run build:css
npm run watch:css
```

`watch:css` watches for changes; stop it with Ctrl+C. The normal `dotnet build`
also compiles CSS and installs the locked npm dependencies if missing. Node.js
and npm are therefore build prerequisites. `dotnet run --no-build` uses the last
compiled CSS. Publish includes the generated file; the deployed server needs no
Node runtime or Tailwind CDN.

The entry point imports Tailwind theme/utilities without Preflight, preserving
the established MVC browser defaults. Shared rules live in the components layer;
utilities written in views can override them. Use existing color/font tokens,
keep responsive/state rules with their component, and use arbitrary utilities
for exact gradients, viewport calculations, and scroll transitions.

Public section headings use university-section-title; content subheadings use
university-subtitle. The university-main scope supplies 17–18px body copy without
changing account/admin forms. Sizes and their semantic line heights live in
base/theme.css; components/typography.css also declares explicit leading to avoid
inherited Tailwind line-height variables changing the intended hierarchy.

Do not modify third-party styles in `wwwroot/lib` or Font Awesome. They are vendor
dependencies, not Ravenclaw's application styles.
