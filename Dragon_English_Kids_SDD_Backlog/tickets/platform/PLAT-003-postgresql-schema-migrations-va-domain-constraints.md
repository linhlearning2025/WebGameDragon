---
id: PLAT-003
title: "PostgreSQL schema, migrations và domain constraints"
kind: feature-spec-ticket
status: DRAFT
spec_approval: required
priority: P0
milestone: M0
domain: platform
dependencies: [PLAT-001]
source: "PRD §3.1–3.6"
stack: [react, phaser-3, dotnet, postgresql]
---

# PLAT-003 — PostgreSQL schema, migrations và domain constraints

> **Spec-driven ticket** · Domain: **Foundation & Architecture** · Priority: **P0** · Release: **M0** · **Trạng thái: DRAFT / chờ xác nhận**

## 1. Business Context — WHY

**User story:** Là backend dev, tôi cần schema bền vững, constraint rõ và migration có thể chạy lại an toàn.

**Nguồn:** PRD §3.1–3.6. Phạm vi được diễn giải theo PRD Dragon English Kids (React + Phaser 3 + .NET Core + PostgreSQL + R2).

## 2. Functional Specification — WHAT

- Tạo 6 bảng PRD, khóa chính/phụ, unique users.email, composite key child_vocab_srs.
- Dùng `JSONB` chỉ cho cấu hình linh hoạt; cột nghiệp vụ quan trọng cần validation.
- FK bảo đảm child_profile → user, dragon_state → child; index trên next_review_at theo child.
- Migrations phải có rollback/forward và không làm mất dữ liệu đã tạo.

## 3. Acceptance Criteria — Given / When / Then

**AC-01**

```gherkin
Scenario: 01 — Luồng chính
Given migration trên DB trống
When apply
Then đủ bảng, unique/FK/check constraints.
```

**AC-02**

```gherkin
Scenario: 02 — Dữ liệu biên
Given insert duplicate email (case-insensitive theo quyết định)
When save
Then nhận conflict.
```

**AC-03**

```gherkin
Scenario: 03 — Bảo vệ và lỗi
Given invalid mastery hoặc evolution level
When save
Then DB hoặc domain từ chối.
```

## 4. Contract & Data Impact

- EF Core migrations; tables `users`, `guest_sessions`, `child_profiles`, `dragon_states`, `vocabularies`, `child_vocab_srs`.
- Indexes dự kiến `(child_id,next_review_at)` và `(tier,topic,difficulty_level)`.

**Quy tắc hợp đồng:** OpenAPI / event payload / schema phải có ví dụ request-response và validation tương ứng trước khi merge. Nội dung **proposed** chưa được xem là API chính thức cho đến khi được review.

## 5. Error Cases & Non-functional Constraints

- Migration lỗi giữa chừng: transaction rollback nếu engine hỗ trợ.

- Tất cả hành động có ghi dữ liệu phải kiểm tra authorization/ownership phía server khi áp dụng.
- Không để UI hiển thị thành công nếu backend chưa xác nhận ghi nhận kết quả.

## 6. Verification — Test Specification

- [ ] Migration up from empty, foreign-key violation, concurrent unique inserts.
- [ ] Mỗi acceptance scenario có automated test hoặc manual test case có evidence; các business rule quan trọng có unit test.
- [ ] Không có regression với luồng Guest → Claim → World Map → Learning → Dragon.

## 7. Implementation Tasks — HOW (đề xuất, điều chỉnh ở bước `/speckit.plan`)

- [ ] Thiết kế ERD
- [ ] EF entities/configurations
- [ ] Migration, indexes
- [ ] Seed test data và tests.

## 8. Dependencies & Boundaries

**Blocked by:** [PLAT-001](../platform/PLAT-001-thiet-lap-monorepo-va-moi-truong-phat-trien.md)

**Không bao gồm:** Các domain khác không được liệt kê ở specification; không mở rộng scope mà không tạo ticket hoặc cập nhật spec.

## 9. Open Decisions / Assumptions

- PRD chưa có bảng stage progress, sessions, answers, reward ledger, donations; bổ sung qua ticket sở hữu domain, không nhồi JSONB.

## 10. Definition of Done (DoD)

- [ ] Product Owner/BA duyệt WHAT + các quyết định đang mở liên quan.
- [ ] API/event/schema được version hóa, cập nhật contract và có consumer/provider tests (nếu áp dụng).
- [ ] Acceptance Criteria AC-01..AC-03 pass; cập nhật bằng chứng test.
- [ ] Security/privacy, quyền dữ liệu trẻ và lỗi/empty/loading state được xác minh phù hợp ticket.
- [ ] Review code, CI xanh, tài liệu và feature flag/rollback strategy khi cần.
