import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { CategoryRequestDto, CategoryResponseDto } from "../interfaces/category.interface";
import { environment } from "../../environments/environment";

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/api/category`;

  getCategories(): Observable<CategoryResponseDto[]> {
    return this.http.get<CategoryResponseDto[]>(this.apiUrl);
  }

  getCategoryById(id: number): Observable<CategoryResponseDto> {
    return this.http.get<CategoryResponseDto>(`${this.apiUrl}/${id}`);
  }

  createCategory(category: CategoryRequestDto): Observable<CategoryResponseDto> {
    return this.http.post<CategoryResponseDto>(this.apiUrl, category);
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  
}
