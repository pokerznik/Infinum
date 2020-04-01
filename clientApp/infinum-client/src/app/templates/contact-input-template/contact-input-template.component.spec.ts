import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContactInputTemplateComponent } from './contact-input-template.component';

describe('ContactInputTemplateComponent', () => {
  let component: ContactInputTemplateComponent;
  let fixture: ComponentFixture<ContactInputTemplateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContactInputTemplateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContactInputTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
