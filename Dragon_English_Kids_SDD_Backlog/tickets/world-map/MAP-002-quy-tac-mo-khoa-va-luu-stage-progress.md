---
id: MAP-002
title: "Quy tắc mở khóa và lưu stage progress"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: world-map
dependencies: [MAP-001, AUTH-003]
source: "PRD current_island_id/current_stage_id; 4 stages"
stack: [react, phaser-3, dotnet, postgresql]
---

# MAP-002 — Quy tắc mở khóa và lưu stage progress

> **Spec-driven ticket** · Domain: **World Map & Assets** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là bé, tôi muốn level mở theo đúng tiến trình và không mất sao đã đạt.

**Nguồn:** PRD current_island_id/current_stage_id; 4 stages. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Server sở hữu state: LOCKED, AVAILABLE, CURRENT, COMPLETED; stage đầu cho phép chơi khi profile hợp lệ.
- Đổi completed chỉ khi stage completion hợp lệ; lưu bestStars=max(current,new), không trừ sao vì replay.
- Prerequisite policy cấu hình (sequential hoặc đủ kỹ năng); không cho client gửi unlocked=true.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given stage đầu tiên
When tạo profile
Then AVAILABLE và hiển thị current.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given hoàn thành stage prerequisite
When recompute
Then stage kế chuyển AVAILABLE.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given replay đạt 1 sao sau lần 3 sao
When save
Then bestStars vẫn 3.
```

## 4. Contract & Data Impact

- Proposed table `child_stage_progress(child_id,stage_id,status,best_stars,last_completed_at)`; progression service.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Concurrent complete/replay: optimistic/transactional update max stars.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Unlock matrix, parallel completion, rollback, replay regression.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Progress model
- [ ] Unlock policy engine
- [ ] Persistence
- [ ] Decision tests.

## 8. Dependencies & Boundaries

**Blocked by:** [MAP-001](../world-map/MAP-001-domain-model-ao-chu-e-va-tuyen-hoc.md), [AUTH-003](../identity/AUTH-003-tao-chinh-sua-ho-so-tre-toi-a-5.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- PRD chưa quy định ngưỡng unlock, số sao và đảo kế; cần PO chốt trước build content.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
