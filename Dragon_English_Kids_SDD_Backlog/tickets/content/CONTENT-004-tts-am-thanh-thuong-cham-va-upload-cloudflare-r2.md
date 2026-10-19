---
id: CONTENT-004
title: "TTS âm thanh thường/chậm và upload Cloudflare R2"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P1
milestone: M2
domain: content
dependencies: [CONTENT-003]
source: "PRD US-4.1; Storage & AI"
stack: [react, phaser-3, dotnet, postgresql]
---

# CONTENT-004 — TTS âm thanh thường/chậm và upload Cloudflare R2

> **Spec-driven ticket** · Domain: **Vocabulary & AI Content** · Priority: **P1** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là content admin, tôi muốn mỗi từ có âm chuẩn tốc độ thường và chậm để game phát nhanh.

**Nguồn:** PRD US-4.1; Storage & AI. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Sinh 2 variant normal/slow theo locale en-US dự kiến; cache fingerprint theo text+voice+speed.
- Upload R2 idempotent với MIME, checksum và key cố định; lưu URLs chỉ sau khi upload/verify thành công.
- Không phát TTS live trên mỗi lần bé chơi; app preload từ CDN.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given word apple chưa có audio
When run job
Then có 2 URL sau upload.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given rerun same config
When fingerprint khớp
Then không tạo file duplicate.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given R2 upload fail
When job kết thúc
Then vocab không ở trạng thái publish-ready.
```

## 4. Contract & Data Impact

- R2 key `vocabulary/{tier}/{vocabId}/{fingerprint}-{normal|slow}.mp3` proposed.
- vocabularies.audio_normal_url/audio_slow_url.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Kiểm tra CORS, CDN TTL, cache headers, provider license.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Checksum and upload tests, retry failure, CDN GET/HEAD validation.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] TTS adapter
- [ ] R2 uploader
- [ ] Fingerprint cache
- [ ] Media verification tests.

## 8. Dependencies & Boundaries

**Blocked by:** [CONTENT-003](../content/CONTENT-003-console-tool-c-sinh-noi-dung-ai-co-kiem-duyet.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt giọng TTS, ngôn ngữ, bitrate, format và quyền lưu trữ âm.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
