---
id: GUEST-004
title: "Thuật toán placement adaptive và phân tầng"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M1
domain: guest
dependencies: [GUEST-003]
source: "PRD US-2.3"
stack: [react, phaser-3, dotnet, postgresql]
---

# GUEST-004 — Thuật toán placement adaptive và phân tầng

> **Spec-driven ticket** · Domain: **Guest Trial & Placement** · Priority: **P0** · Release: **M1** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là hệ thống, tôi cần thay đổi độ khó theo chuỗi kết quả, sau đó gợi ý STARTERS/MOVERS có thể giải thích.

**Nguồn:** PRD US-2.3. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Difficulty khởi đầu dựa age là tín hiệu phụ, giới hạn 1–3.
- Tăng/giảm difficulty từ chuỗi đúng/sai theo rule cấu hình và audit được.
- Kết quả ghi tier, score, answeredCount và ruleVersion, không thay thế đánh giá giáo viên.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given 2 đáp án đúng liên tiếp
When chọn câu kế
Then difficulty tăng tối đa 3 theo policy.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given 2 đáp án sai liên tiếp
When chọn câu kế
Then difficulty giảm tối thiểu 1.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given 6–8 câu hoàn tất
When classify
Then tier được suy ra bởi threshold có version.
```

## 4. Contract & Data Impact

- Pure function `nextDifficulty(history, policy)`; response `tier`, `confidenceBand`, `ruleVersion` proposed.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Thiếu câu hỏi đúng difficulty: fallback gần nhất và ghi telemetry.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Decision table, min/max bounds, reproducibility tests.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Thiết kế rules file
- [ ] Implement engine
- [ ] Golden test cases
- [ ] Learning signoff.

## 8. Dependencies & Boundaries

**Blocked by:** [GUEST-003](../guest/GUEST-003-ngan-hang-cau-hoi-placement-va-ghi-nhan-cau-tra-loi.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- Ngưỡng STARTERS/MOVERS, luật streak cụ thể là giả định cần PO/learning designer phê duyệt.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
