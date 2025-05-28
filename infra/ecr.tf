resource "aws_ecr_repository" "quiosque_repo" {
  name                 = "quiosquefood3000"
  image_tag_mutability = "MUTABLE"

  lifecycle_policy {
    policy = jsonencode({
      rules = [
        {
          rulePriority = 1
          description  = "Expire untagged images older than 30 days"
          selection    = {
            tagStatus = "untagged"
            countType = "sinceImagePushed"
            countUnit = "days"
            countNumber = 30
          }
          action = {
            type = "expire"
          }
        }
      ]
    })
  }
}
