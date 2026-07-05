# Railway IaC configuration

This folder manages Railway infrastructure as code using the file `railway.ts`.

## Where to run commands

Run all commands from this folder:

```bash
cd /workspaces/padylife/.railway
```

## Script-based workflow

Use npm scripts in `package.json` instead of raw `railway config` commands.
These scripts use the correct IaC file and runner automatically.

Preview changes:

```bash
npm run plan:production
npm run plan:staging
```

Apply changes:

```bash
npm run apply:production
npm run apply:staging
```

Apply both environments:

```bash
npm run apply:all
```

List domains managed per service:

```bash
npm run domains:list
```

## What these scripts do

- Select target environment (`production` or `staging`).
- Run plan/apply with:
	- `--file railway.ts`
	- `--runner ./node_modules/.bin/railway-iac-ts`
- Apply non-interactively with destructive confirmation:
	- `--yes --confirm-destructive`
- `apply:all` runs production + staging apply and restores linked env to production.
- `domains:list` prints current domain bindings for `api`, `app`, `admin`, and `www` in both environments.

## Notes

- `plan` is read-only and safe.
- `apply` changes live Railway resources.
- If you see unexpected deletes in plan output, stop and review before applying.
- Keep all infrastructure changes in `railway.ts` so environments stay consistent.

## Troubleshooting drift

If `plan` still shows changes immediately after `apply`, use this checklist:

1. Confirm the right environment is linked before running plan/apply.
2. Always run scripts from this folder so the correct runner and file are used.
3. Re-run `plan` and compare repeated fields (for example: build command, root directory, variables, or old resources marked for delete).
4. If the same resources keep reappearing, verify there is no parallel manual config in Railway dashboard fighting IaC values.
5. If old database or volume resources keep returning in plan, apply once with destructive confirmation already included in this repo scripts, then run plan again.

Quick verify loop:

```bash
npm run plan:production
npm run apply:production
npm run plan:production

npm run plan:staging
npm run apply:staging
npm run plan:staging

npm run domains:list
```

If drift persists after this loop, run JSON plan output and inspect exact fields:

```bash
railway environment production
railway config plan --file railway.ts --runner ./node_modules/.bin/railway-iac-ts --json

railway environment staging
railway config plan --file railway.ts --runner ./node_modules/.bin/railway-iac-ts --json
```
