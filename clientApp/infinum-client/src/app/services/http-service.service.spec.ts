import { TestBed } from '@angular/core/testing';


describe('HttpServiceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: HttpServiceService = TestBed.get(HttpServiceService);
    expect(service).toBeTruthy();
  });
});
