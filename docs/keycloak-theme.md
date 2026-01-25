# Keycloak theme (spendi-meno-semo)

This repo includes a custom Keycloak login theme matching the app UI.

## Files
- `keycloak/themes/spendi-meno-semo/login/theme.properties`
- `keycloak/themes/spendi-meno-semo/login/resources/css/login.css`

Note: for Keycloak 21, the theme uses `parent=keycloak`.

## Enable in Docker
The `docker-compose.yml` should mount `./keycloak/themes` into the Keycloak container at `/opt/keycloak/themes`.

Then restart Keycloak.

## Enable in Keycloak Admin Console
1. Open `http://localhost:8080/admin`
2. Select realm `myapp`
3. Go to **Realm settings** → **Themes**
4. Set **Login theme** = `spendi-meno-semo`
5. Save

After saving, open your app again; the Keycloak login page will use the new theme.

If Keycloak caches the theme, restart the container.

## Troubleshooting

- If the theme does not appear in the dropdown, check container logs for theme load errors.
- If changes to CSS do not show up, do a hard refresh (Ctrl+F5) and restart the `keycloak` container.
