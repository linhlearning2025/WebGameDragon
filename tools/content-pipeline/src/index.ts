const command = process.argv[2] ?? "help";

if (command === "help") {
  process.stdout.write(
    "Dragon English Kids content pipeline\nUsage: pnpm dev:content [help]\n",
  );
  process.exit(0);
}

process.stderr.write(`Unknown content-pipeline command: ${command}\n`);
process.exitCode = 2;
