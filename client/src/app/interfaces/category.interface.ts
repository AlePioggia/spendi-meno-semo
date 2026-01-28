export interface CategoryRequestDto {
  name: string;
  description: string;
}

export interface CategoryResponseDto {
  id: number;
  name: string;
  description: string;
  userId: number;
  tenantId: number;
  createdAt: Date;
}
