import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GradesRoutingModule } from './grades-routing.module';
import { GradeViewComponent } from './grade-view/grade-view.component';
import { ClassGradesComponent } from './class-grades/class-grades.component';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    GradesRoutingModule,
    GradeViewComponent,
    ClassGradesComponent,
  ],
})
export class GradesModule {}
