---
description: Daily Semgrep security scan for SQL injection and other vulnerabilities
name: Daily Semgrep Scan
imports:
  - shared/mcp/semgrep.md
on:
  schedule: daily
  workflow_dispatch:
permissions:
  contents: read
  issues: read
  pull-requests: read
  security-events: read
safe-outputs:
  create-code-scanning-alert:
    driver: "Semgrep Security Scanner"
source: github/gh-aw/.github/workflows/daily-semgrep-scan.md@f19b6f91dfc7dd02687de50d53b3355cc4f64a63
---

Scan the repository for SQL injection vulnerabilities using Semgrep.

**Important**: If no action is needed after completing your analysis, you **MUST** call the `noop` safe-output tool with a brief explanation. Failing to call any safe-output tool is the most common cause of safe-output workflow failures.

```json
{"noop": {"message": "No action needed: [brief explanation of what was analyzed and why]"}}
```
