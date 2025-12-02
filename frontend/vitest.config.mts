import { defineConfig } from "vitest/config"
import react from "@vitejs/plugin-react"
import tsconfigPaths from "vite-tsconfig-paths"

export default defineConfig({
    plugins: [tsconfigPaths(), react()],
    test: {
        environment: "jsdom",
        setupFiles: ["./src/tests/setup.ts"],
        exclude: [
            "node_modules/**",
            "**/e2e/**",
            "**/*.spec.ts",
        ],
        coverage: {
            provider: "v8",
            reporter: ["text", "json", "html"],
            exclude: [
                "node_modules/",
                "src/tests/",
                "**/*.config.*",
                "**/*.d.ts",
            ],
        },
    }
})