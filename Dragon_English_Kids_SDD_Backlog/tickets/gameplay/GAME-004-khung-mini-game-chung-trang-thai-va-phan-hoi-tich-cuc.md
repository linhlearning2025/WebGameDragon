---
id: GAME-004
title: "Khung mini-game chung, trạng thái và phản hồi tích cực"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M2
domain: gameplay
dependencies: [PLAT-002, GAME-001]
source: "PRD US-3.2–3.5; no red X"
stack: [react, phaser-3, dotnet, postgresql]
---

# GAME-004 — Khung mini-game chung, trạng thái và phản hồi tích cực

> **Spec-driven ticket** · Domain: **Mini-games & Reward** · Priority: **P0** · Release: **M2** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn trải nghiệm nhất quán giữa bốn loại bài.

**Nguồn:** PRD US-3.2–3.5; no red X. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Common lifecycle preload→intro→question→feedback→next→summary; pause/resume và audio replay.
- UI đồng nhất nút Home/Back, sound, progress, feedback an toàn; không có dấu X đỏ khi sai.
- Một câu chỉ được tính một lần, transition không chạy đồng thời do double click.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given bất kỳ stage skill
When bắt đầu
Then có title, audio controls và progress.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given trả lời sai
When feedback
Then hiển thị gợi ý/động viên không phạt.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given app tab hidden
When trở lại
Then không bỏ qua câu do timer canvas.
```

## 4. Contract & Data Impact

- `BaseMiniGameScene` hoặc composable engine; `StageQuestion` discriminated DTO proposed.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Media fail: retry và fallback text/illustration nếu được phép bởi learning objective.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Scene state machine, double tap, audio/touch, pause/resume.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Common scene components
- [ ] Input lock
- [ ] State machine
- [ ] E2E.

## 8. Dependencies & Boundaries

**Blocked by:** [PLAT-002](../platform/PLAT-002-event-bridge-react-phaser-co-kieu-du-lieu.md), [GAME-001](../gameplay/GAME-001-stage-data-api-tron-70-moi-30-on-srs.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt text/font kích cỡ, narration và accessibility behavior.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
