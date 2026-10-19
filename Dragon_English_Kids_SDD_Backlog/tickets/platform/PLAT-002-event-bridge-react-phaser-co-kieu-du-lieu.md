---
id: PLAT-002
title: "Event Bridge React ↔ Phaser có kiểu dữ liệu"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M0
domain: platform
dependencies: [PLAT-001]
source: "PRD §1 React-Phaser Event Bridge"
stack: [react, phaser-3, dotnet, postgresql]
---

# PLAT-002 — Event Bridge React ↔ Phaser có kiểu dữ liệu

> **Spec-driven ticket** · Domain: **Foundation & Architecture** · Priority: **P0** · Release: **M0** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là frontend dev, tôi muốn hai lớp React/Phaser giao tiếp có hợp đồng sự kiện rõ ràng mà không trộn UI phụ huynh vào game.

**Nguồn:** PRD §1 React-Phaser Event Bridge. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Định nghĩa event namespace `dragon:*` và payload schema có version.
- React quản lý authentication, modal, permission, parent UI; Phaser sở hữu map/gameplay/render.
- Mọi listener đăng ký khi mount và hủy khi unmount, không nhận event trùng.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given Phaser phát `dragon:level-selected`
When React nhận
Then biết `childId` và `stageId` hợp lệ.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given React chuyển profile
When event `dragon:profile-changed` gửi
Then Phaser refresh progress đúng profile.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given component unmount/remount
When phát một event
Then callback thực thi đúng một lần.
```

## 4. Contract & Data Impact

- Events proposed: level-selected{childId,stageId}; stage-start; stage-exit; progress-updated; mic-permission-status.
- Shared TypeScript discriminated union và schema validation phía boundary.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Payload sai hoặc stale childId: bỏ qua có telemetry; không điều hướng.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Unit event contract; integration mount/unmount; profile isolation.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Khai báo typed events
- [ ] Làm bridge adapter
- [ ] Test lifecycle và payload.

## 8. Dependencies & Boundaries

**Blocked by:** [PLAT-001](../platform/PLAT-001-thiet-lap-monorepo-va-moi-truong-phat-trien.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Chốt event transport (EventTarget/EventEmitter) và versioning.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
