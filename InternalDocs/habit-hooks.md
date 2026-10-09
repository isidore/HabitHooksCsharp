Run `tools/run-habit-hooks.sh` from the repo root. It rebuilds/reinstalls the local
`../../HabitHooksCsharp` plugin, then runs `habit-hooks` against the C# backend.

- `./tools/run-habit-hooks.sh <file.cs>` — check one file (pass any path that exists)
- `./tools/run-habit-hooks.sh <file.js|.ts|.vue>` — check one frontend file (runs from `frontend/student-portal`)
- `./tools/run-habit-hooks.sh --branch main` — check files changed vs. main
- `./tools/run-habit-hooks.sh --all` — check the whole backend
- `./tools/run-habit-hooks.sh` — default scope from `backend/.habit-hooks/config.toml`

## Frontend (TypeScript/Vue)

The install also includes the `habit-hooks-typescript` plugin, used by
`frontend/student-portal/.habit-hooks/config.toml`. Its comment sensor needs
`ts-morph` as a dev dependency in `frontend/student-portal` (already in
`package.json`). Run from that directory:

- `habit-hooks --file src/App.vue` — check one file
- `habit-hooks --branch main` — check files changed vs. main

The eslint sensor runs with `eslint.habit-hooks.config.js` (set in the
`[sensors.eslint]` args), which layers the habit-hooks thresholds (function
length, params, complexity, nesting, ...) on top of the project's
`eslint.config.js`. `npm run lint` uses only `eslint.config.js`.

Known gap: the plugin's eslint sensor skips `.vue` files, so only the comment,
file-size and duplication smells reach a view's `<script>` today. Preview what it
will report with `npx eslint --config eslint.habit-hooks.config.js src/views`.
