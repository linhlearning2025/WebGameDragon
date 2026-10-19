---
id: QA-004
title: "Hiệu năng âm thanh, preload và R2 CDN"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P1
milestone: M2
domain: quality
dependencies: [CONTENT-004, GAME-005, ASSET-001]
source: "PRD §1 audio dưới 100ms sau touch"
stack: [react, phaser-3, dotnet, postgresql]
---

# QA-004 — Hiệu năng âm thanh, preload và R2 CDN

> **Spec-driven ticket** · Domain: **Quality, Privacy & Release** · Priority: **P1** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn chạm phát từ vựng gần như ngay lập tức.

**Nguồn:** PRD §1 audio dưới 100ms sau touch. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Mục tiêu <100 ms đo từ user tap đến audio playback start **khi audio đã preload/decoded** trên thiết bị tham chiếu.
- Preload assets câu hiện tại+tiếp theo, budget bộ nhớ và fallback low bandwidth.
- Lập metric p50/p95 trên mobile/desktop; phân biệt cached và uncached, không hứa 100ms khi mạng chưa tải.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given cached audio và browser unlock audio context
When tap
Then p95 target <100ms trên test devices đã chốt.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given uncached/slow network
When request
Then loading prompt và không miss câu hỏi.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given asset 404
When preload
Then hiện retry/fallback không crash.
```

## 4. Contract & Data Impact

- Performance profile, asset size/codec budget, CDN Cache-Control, observability metrics.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Browser autoplay policy: phải unlock qua user gesture.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Lighthouse/Web Vitals + scripted audio tap latency, network throttle, cache test.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Instrumentation
- [ ] Preloading
- [ ] Load tests
- [ ] Optimization.

## 8. Dependencies & Boundaries

**Blocked by:** [CONTENT-004](../content/CONTENT-004-tts-am-thanh-thuong-cham-va-upload-cloudflare-r2.md), [GAME-005](../gameplay/GAME-005-listening-rong-op-may-ung-hinh.md), [ASSET-001](../world-map/ASSET-001-chuan-asset-manifest-va-preload-theo-phien-ban.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt benchmark devices và p95 threshold production; PRD nói “dưới 100ms” nhưng không đủ điều kiện mạng.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
