import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GradeViewComponent } from './grade-view/grade-view.component';
import { ClassGradesComponent } from './class-grades/class-grades.component';

const routes: Routes = [
  { path: '', component: GradeViewComponent },
  { path: ':id', component: ClassGradesComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GradesRoutingModule {}
