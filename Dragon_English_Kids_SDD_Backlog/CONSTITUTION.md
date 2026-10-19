# Project Constitution — Dragon English Kids

**Status:** DRAFT — must be approved before implementation. **Scope:** React + Phaser 3 + .NET Core + PostgreSQL + Cloudflare R2, trẻ 5–8 tuổi.

## Product principles (normative when approved)

1. **Child-first UX:** khuyến khích học tập; không dùng dấu X đỏ, pressure/shame hoặc paywall với kiến thức cốt lõi.
2. **One source of truth:** .NET Core chịu trách nhiệm auth, unlock, scores, SRS, inventory/EXP; Phaser là visual/gameplay client, không tự xác nhận reward.
3. **Domain ownership:** Identity owns parents/children, Guest owns trial, Content owns words/media, Learning owns attempts/SRS, Progress owns unlock, Dragon owns inventory/EXP, Parent Reporting owns aggregates.
4. **Privacy by design:** giới hạn PII, parent consent và micro permission; không lưu raw audio/transcripts mặc định; PIN băm, tránh log dữ liệu nhạy cảm.
5. **Spec-before-code:** mọi ticket phải có WHAT, scenario Given/When/Then, contracts và quyết định đã được PO duyệt trước khi bước implement.
6. **Idempotency and atomicity:** claim, completion, feeding, donation confirmation không được ghi trùng khi retry hoặc double click.
7. **Backward compatibility:** schema/API/event có version; không tự ý thay đổi consumer contract trong cùng release.
8. **Accessibility:** touch targets đủ lớn, reduced motion, fallback audio/mic, hoạt động trên desktop/tablet/mobile.
9. **Content quality:** học liệu/TTS/AI phải qua schema, review và publishing gate; không tự publish AI output.
10. **Observability:** errors có traceId, tests CI, không log tokens/PIN/voice/raw data trẻ.

## Tech guardrails

- React DOM: auth/profile/parent dashboard/modals/HUD.
- Phaser: world map, animations, mini-games; event bridge typed+unsubscribed.
- PostgreSQL + EF Core: migration/schema/integrity. JSONB chỉ dành cho metadata và cấu hình biến đổi.
- R2: immutable versioned assets, CORS/CDN/cache; audio preload client.
- Never trust client-reported `isCorrect`, stars, rewards, mastery, unlock or supporter status.

## Quality gates

Specification drafted → ambiguities resolved → contract reviewed → task decomposition → implementation → tests → convergence against acceptance criteria.

This layout is **inspired by** Spec-Driven Development / GitHub Spec Kit, but is not a drop-in replacement for the entire `specify` CLI's generated feature directories. Each ticket contains its own spec and initial task checklist to facilitate independent execution.
