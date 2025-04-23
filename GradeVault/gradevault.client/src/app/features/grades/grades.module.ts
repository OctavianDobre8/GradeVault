import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GradesRoutingModule } from './grades-routing.module';
import { GradeViewComponent } from './grade-view/grade-view.component';

@NgModule({
  declarations: [],
  imports: [CommonModule, GradesRoutingModule, GradeViewComponent],
})
export class GradesModule {}
