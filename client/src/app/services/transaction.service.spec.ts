import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../../environments/environment';
import { TransactionService } from './transaction.service';

describe('TransactionService', () => {
  let service: TransactionService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    service = TestBed.inject(TransactionService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should request the report as a blob', () => {
    const blob = new Blob(['report'], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    let result: Blob | null = null;

    service.downloadReport().subscribe((response: Blob) => {
      result = response;
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/api/transaction/report`);
    expect(req.request.method).toBe('GET');
    expect(req.request.responseType).toBe('blob');

    req.flush(blob);

    expect(result).toBe(blob);
  });
});
